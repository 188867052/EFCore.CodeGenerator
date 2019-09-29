using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EFCore.Scaffolding.Extension;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public class RelationalScaffoldingModelFactoryBase : IScaffoldingModelFactory
    {
        internal const string NavigationNameUniquifyingPattern = "{0}Navigation";
        internal const string SelfReferencingPrincipalEndNavigationNamePattern = "Inverse{0}";

        private readonly IOperationReporter _reporter;
        private readonly ICandidateNamingService _candidateNamingService;
        private Dictionary<DatabaseTable, CSharpUniqueNamer<DatabaseColumn>> _columnNamers;
        private bool _useDatabaseNames;
        private readonly DatabaseTable _nullTable = new DatabaseTable();
        private CSharpUniqueNamer<DatabaseTable> _tableNamer;
        private CSharpUniqueNamer<DatabaseTable> _dbSetNamer;
        private readonly HashSet<DatabaseColumn> _unmappedColumns = new HashSet<DatabaseColumn>();
        private readonly IPluralizer _pluralizer;
        private readonly ICSharpUtilities _cSharpUtilities;
        private readonly IScaffoldingTypeMapper _scaffoldingTypeMapper;
        private readonly LoggingDefinitions _loggingDefinitions;

        public RelationalScaffoldingModelFactoryBase(
            [NotNull] IOperationReporter reporter,
            [NotNull] ICandidateNamingService candidateNamingService,
            [NotNull] IPluralizer pluralizer,
            [NotNull] ICSharpUtilities cSharpUtilities,
            [NotNull] IScaffoldingTypeMapper scaffoldingTypeMapper,
            [NotNull] LoggingDefinitions loggingDefinitions)
        {
            Check.NotNull(reporter, nameof(reporter));
            Check.NotNull(candidateNamingService, nameof(candidateNamingService));
            Check.NotNull(pluralizer, nameof(pluralizer));
            Check.NotNull(cSharpUtilities, nameof(cSharpUtilities));
            Check.NotNull(scaffoldingTypeMapper, nameof(scaffoldingTypeMapper));
            Check.NotNull(loggingDefinitions, nameof(loggingDefinitions));

            this._reporter = reporter;
            this._candidateNamingService = candidateNamingService;
            this._pluralizer = pluralizer;
            this._cSharpUtilities = cSharpUtilities;
            this._scaffoldingTypeMapper = scaffoldingTypeMapper;
            this._loggingDefinitions = loggingDefinitions;
        }

        public virtual IModel Create(DatabaseModel databaseModel, bool useDatabaseNames)
        {
            Check.NotNull(databaseModel, nameof(databaseModel));

            var modelBuilder = new ModelBuilder(new ConventionSet());

            this._tableNamer = new CSharpUniqueNamer<DatabaseTable>(
                useDatabaseNames
                    ? (Func<DatabaseTable, string>)(t => t.Name)
                    : t => this._candidateNamingService.GenerateCandidateIdentifier(t),
                this._cSharpUtilities,
                useDatabaseNames
                    ? (Func<string, string>)null
                    : this._pluralizer.Singularize);
            this._dbSetNamer = new CSharpUniqueNamer<DatabaseTable>(
                useDatabaseNames
                    ? (Func<DatabaseTable, string>)(t => t.Name)
                    : t => this._candidateNamingService.GenerateCandidateIdentifier(t),
                this._cSharpUtilities,
                useDatabaseNames
                    ? (Func<string, string>)null
                    : this._pluralizer.Pluralize);
            this._columnNamers = new Dictionary<DatabaseTable, CSharpUniqueNamer<DatabaseColumn>>();
            this._useDatabaseNames = useDatabaseNames;

            this.VisitDatabaseModel(modelBuilder, databaseModel);

            return modelBuilder.Model;
        }

        protected virtual string GetEntityTypeName([NotNull] DatabaseTable table)
            => this._tableNamer.GetName(Check.NotNull(table, nameof(table)));

        protected virtual string GetDbSetName([NotNull] DatabaseTable table)
            => this._dbSetNamer.GetName(Check.NotNull(table, nameof(table)));

        protected virtual string GetPropertyName([NotNull] DatabaseColumn column)
        {
            Check.NotNull(column, nameof(column));

            var table = column.Table ?? this._nullTable;
            var usedNames = new List<string>();
            if (column.Table != null)
            {
                usedNames.Add(this.GetEntityTypeName(table));
            }

            if (!this._columnNamers.ContainsKey(table))
            {
                if (this._useDatabaseNames)
                {
                    this._columnNamers.Add(
                        table,
                        new CSharpUniqueNamer<DatabaseColumn>(
                            c => c.Name,
                            usedNames,
                            this._cSharpUtilities,
                            singularizePluralizer: null));
                }
                else
                {
                    this._columnNamers.Add(
                        table,
                        new CSharpUniqueNamer<DatabaseColumn>(
                            c => this._candidateNamingService.GenerateCandidateIdentifier(c),
                            usedNames,
                            this._cSharpUtilities,
                            singularizePluralizer: null));
                }
            }

            return this._columnNamers[table].GetName(column);
        }

        protected virtual ModelBuilder VisitDatabaseModel([NotNull] ModelBuilder modelBuilder, [NotNull] DatabaseModel databaseModel)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(databaseModel, nameof(databaseModel));

            if (!string.IsNullOrEmpty(databaseModel.DefaultSchema))
            {
                modelBuilder.HasDefaultSchema(databaseModel.DefaultSchema);
            }

            if (!string.IsNullOrEmpty(databaseModel.DatabaseName))
            {
                modelBuilder.Model.SetDatabaseName(databaseModel.DatabaseName);
            }

            this.VisitSequences(modelBuilder, databaseModel.Sequences);
            this.VisitTables(modelBuilder, databaseModel.Tables);
            this.VisitForeignKeys(modelBuilder, databaseModel.Tables.SelectMany(table => table.ForeignKeys).ToList());

            modelBuilder.Model.AddAnnotations(databaseModel.GetAnnotations());

            return modelBuilder;
        }

        protected virtual ModelBuilder VisitSequences([NotNull] ModelBuilder modelBuilder, [NotNull] ICollection<DatabaseSequence> sequences)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(sequences, nameof(sequences));

            foreach (var sequence in sequences)
            {
                this.VisitSequence(modelBuilder, sequence);
            }

            return modelBuilder;
        }

        protected virtual SequenceBuilder VisitSequence([NotNull] ModelBuilder modelBuilder, [NotNull] DatabaseSequence sequence)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(sequence, nameof(sequence));

            if (string.IsNullOrEmpty(sequence.Name))
            {
                this._reporter.WriteWarning(DesignStrings.SequencesRequireName);
                return null;
            }

            Type sequenceType = null;
            if (sequence.StoreType != null)
            {
                sequenceType = this._scaffoldingTypeMapper.FindMapping(
                        sequence.StoreType,
                        keyOrIndex: false,
                        rowVersion: false)
                    ?.ClrType;
            }

            if (sequenceType != null
                && !Sequence.SupportedTypes.Contains(sequenceType))
            {
                this._reporter.WriteWarning(DesignStrings.BadSequenceType(sequence.Name, sequence.StoreType));
                return null;
            }

            var builder = sequenceType != null
                ? modelBuilder.HasSequence(sequenceType, sequence.Name, sequence.Schema)
                : modelBuilder.HasSequence(sequence.Name, sequence.Schema);

            if (sequence.IncrementBy.HasValue)
            {
                builder.IncrementsBy(sequence.IncrementBy.Value);
            }

            if (sequence.MaxValue.HasValue)
            {
                builder.HasMax(sequence.MaxValue.Value);
            }

            if (sequence.MinValue.HasValue)
            {
                builder.HasMin(sequence.MinValue.Value);
            }

            if (sequence.StartValue.HasValue)
            {
                builder.StartsAt(sequence.StartValue.Value);
            }

            if (sequence.IsCyclic.HasValue)
            {
                builder.IsCyclic(sequence.IsCyclic.Value);
            }

            return builder;
        }

        protected virtual ModelBuilder VisitTables([NotNull] ModelBuilder modelBuilder, [NotNull] ICollection<DatabaseTable> tables)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(tables, nameof(tables));

            foreach (var table in tables)
            {
                this.VisitTable(modelBuilder, table);
            }

            return modelBuilder;
        }

        protected virtual void SetTableDictionary(string entityTypeName, DatabaseTable table)
        {
        }

        protected virtual void SetKeyDictionary(KeyBuilder keyBuilder, DatabaseTable table)
        {
        }

        protected virtual EntityTypeBuilder VisitTable([NotNull] ModelBuilder modelBuilder, [NotNull] DatabaseTable table)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(table, nameof(table));

            var entityTypeName = this.GetEntityTypeName(table);
            this.SetTableDictionary(entityTypeName, table);
            var builder = modelBuilder.Entity(entityTypeName);

            var dbSetName = this.GetDbSetName(table);
            builder.Metadata.SetDbSetName(dbSetName);

            if (table is DatabaseView)
            {
                builder.ToView(table.Name, table.Schema);
            }
            else
            {
                builder.ToTable(table.Name, table.Schema);
            }

            if (table.Comment != null)
            {
                builder.HasComment(table.Comment);
            }

            this.VisitColumns(builder, table.Columns);

            if (table.PrimaryKey != null)
            {
                var keyBuilder = this.VisitPrimaryKey(builder, table);

                this.SetKeyDictionary(keyBuilder, table);

                if (keyBuilder == null)
                {
                    var errorMessage = DesignStrings.UnableToGenerateEntityType(table.DisplayName());
                    this._reporter.WriteWarning(errorMessage);

                    var model = modelBuilder.Model;
                    model.RemoveEntityType(entityTypeName);
                    model.GetEntityTypeErrors().Add(entityTypeName, errorMessage);
                    return null;
                }
            }

            this.VisitUniqueConstraints(builder, table.UniqueConstraints);
            this.VisitIndexes(builder, table.Indexes);

            builder.Metadata.AddAnnotations(table.GetAnnotations());

            return builder;
        }

        protected virtual EntityTypeBuilder VisitColumns([NotNull] EntityTypeBuilder builder, [NotNull] ICollection<DatabaseColumn> columns)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(columns, nameof(columns));

            foreach (var column in columns)
            {
                this.VisitColumn(builder, column);
            }

            return builder;
        }

        protected virtual void SetDictionary(PropertyBuilder property, DatabaseColumn column)
        {
        }

        protected virtual PropertyBuilder VisitColumn([NotNull] EntityTypeBuilder builder, [NotNull] DatabaseColumn column)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(column, nameof(column));

            var typeScaffoldingInfo = this.GetTypeScaffoldingInfo(column);

            if (typeScaffoldingInfo == null)
            {
                this._unmappedColumns.Add(column);
                this._reporter.WriteWarning(
                    DesignStrings.CannotFindTypeMappingForColumn(column.DisplayName(), column.StoreType));
                return null;
            }

            var clrType = typeScaffoldingInfo.ClrType;
            if (column.IsNullable)
            {
                clrType = clrType.MakeNullable();
            }

            if (clrType == typeof(bool)
                && column.DefaultValueSql != null)
            {
                this._reporter.WriteWarning(
                    DesignStrings.NonNullableBoooleanColumnHasDefaultConstraint(column.DisplayName()));

                clrType = clrType.MakeNullable();
            }

            var property = builder.Property(clrType, this.GetPropertyName(column));

            property.HasColumnName(column.Name);

            if (!typeScaffoldingInfo.IsInferred
                && !string.IsNullOrWhiteSpace(column.StoreType))
            {
                property.HasColumnType(column.StoreType);
            }

            if (typeScaffoldingInfo.ScaffoldUnicode.HasValue)
            {
                property.IsUnicode(typeScaffoldingInfo.ScaffoldUnicode.Value);
            }

            if (typeScaffoldingInfo.ScaffoldFixedLength == true)
            {
                property.IsFixedLength();
            }

            if (typeScaffoldingInfo.ScaffoldMaxLength.HasValue)
            {
                property.HasMaxLength(typeScaffoldingInfo.ScaffoldMaxLength.Value);
            }

            if (column.ValueGenerated == ValueGenerated.OnAdd)
            {
                property.ValueGeneratedOnAdd();
            }

            if (column.ValueGenerated == ValueGenerated.OnUpdate)
            {
                property.ValueGeneratedOnUpdate();
            }

            if (column.ValueGenerated == ValueGenerated.OnAddOrUpdate)
            {
                property.ValueGeneratedOnAddOrUpdate();
            }

            if (column.DefaultValueSql != null)
            {
                property.HasDefaultValueSql(column.DefaultValueSql);
            }

            if (column.ComputedColumnSql != null)
            {
                property.HasComputedColumnSql(column.ComputedColumnSql);
            }

            if (column.Comment != null)
            {
                property.HasComment(column.Comment);
            }

            if (!(column.Table.PrimaryKey?.Columns.Contains(column) ?? false))
            {
                property.IsRequired(!column.IsNullable);
            }

            if ((bool?)column[ScaffoldingAnnotationNames.ConcurrencyToken] == true)
            {
                property.IsConcurrencyToken();
            }

            property.Metadata.SetColumnOrdinal(column.Table.Columns.IndexOf(column));

            property.Metadata.AddAnnotations(
                column.GetAnnotations().Where(
                    a => a.Name != ScaffoldingAnnotationNames.ConcurrencyToken));

            this.SetDictionary(property, column);
            return property;
        }

        protected virtual KeyBuilder VisitPrimaryKey([NotNull] EntityTypeBuilder builder, [NotNull] DatabaseTable table)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(table, nameof(table));

            var primaryKey = table.PrimaryKey;

            var unmappedColumns = primaryKey.Columns
                .Where(c => this._unmappedColumns.Contains(c))
                .Select(c => c.Name)
                .ToList();
            if (unmappedColumns.Count > 0)
            {
                this._reporter.WriteWarning(
                    DesignStrings.PrimaryKeyErrorPropertyNotFound(
                        table.DisplayName(),
                        string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, unmappedColumns)));
                return null;
            }

            var keyBuilder = builder.HasKey(primaryKey.Columns.Select(this.GetPropertyName).ToArray());


            if (primaryKey.Columns.Count == 1
                && primaryKey.Columns[0].ValueGenerated == null
                && primaryKey.Columns[0].DefaultValueSql == null)
            {
                var property = builder.Metadata.FindProperty(this.GetPropertyName(primaryKey.Columns[0]))?.AsProperty();
                if (property != null)
                {
                    var conventionalValueGenerated = RelationalValueGenerationConvention.GetValueGenerated(property);
                    if (conventionalValueGenerated == ValueGenerated.OnAdd)
                    {
                        property.ValueGenerated = ValueGenerated.Never;
                    }
                }
            }

            if (!string.IsNullOrEmpty(primaryKey.Name)
                && primaryKey.Name != keyBuilder.Metadata.GetDefaultName())
            {
                keyBuilder.HasName(primaryKey.Name);
            }

            keyBuilder.Metadata.AddAnnotations(primaryKey.GetAnnotations());

            return keyBuilder;
        }

        protected virtual EntityTypeBuilder VisitUniqueConstraints([NotNull] EntityTypeBuilder builder, [NotNull] ICollection<DatabaseUniqueConstraint> uniqueConstraints)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(uniqueConstraints, nameof(uniqueConstraints));

            foreach (var uniqueConstraint in uniqueConstraints)
            {
                this.VisitUniqueConstraint(builder, uniqueConstraint);
            }

            return builder;
        }

        protected virtual IndexBuilder VisitUniqueConstraint([NotNull] EntityTypeBuilder builder, [NotNull] DatabaseUniqueConstraint uniqueConstraint)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(uniqueConstraint, nameof(uniqueConstraint));

            var unmappedColumns = uniqueConstraint.Columns
                .Where(c => this._unmappedColumns.Contains(c))
                .Select(c => c.Name)
                .ToList();
            if (unmappedColumns.Count > 0)
            {
                this._reporter.WriteWarning(
                    DesignStrings.UnableToScaffoldIndexMissingProperty(
                        uniqueConstraint.Name,
                        string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, unmappedColumns)));
                return null;
            }

            var propertyNames = uniqueConstraint.Columns.Select(this.GetPropertyName).ToArray();
            var indexBuilder = builder.HasIndex(propertyNames).IsUnique();

            if (!string.IsNullOrEmpty(uniqueConstraint.Name)
                && uniqueConstraint.Name != indexBuilder.Metadata.GetDefaultName())
            {
                indexBuilder.HasName(uniqueConstraint.Name);
            }

            indexBuilder.Metadata.AddAnnotations(uniqueConstraint.GetAnnotations());

            return indexBuilder;
        }

        protected virtual EntityTypeBuilder VisitIndexes([NotNull] EntityTypeBuilder builder, [NotNull] ICollection<DatabaseIndex> indexes)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(indexes, nameof(indexes));

            foreach (var index in indexes)
            {
                this.VisitIndex(builder, index);
            }

            return builder;
        }

        protected virtual IndexBuilder VisitIndex([NotNull] EntityTypeBuilder builder, [NotNull] DatabaseIndex index)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(index, nameof(index));

            var unmappedColumns = index.Columns
                .Where(c => this._unmappedColumns.Contains(c))
                .Select(c => c.Name)
                .ToList();
            if (unmappedColumns.Count > 0)
            {
                this._reporter.WriteWarning(
                    DesignStrings.UnableToScaffoldIndexMissingProperty(
                        index.Name,
                        string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, unmappedColumns)));
                return null;
            }

            var propertyNames = index.Columns.Select(this.GetPropertyName).ToArray();
            var indexBuilder = builder.HasIndex(propertyNames)
                .IsUnique(index.IsUnique);

            if (index.Filter != null)
            {
                indexBuilder.HasFilter(index.Filter);
            }

            if (!string.IsNullOrEmpty(index.Name)
                && index.Name != indexBuilder.Metadata.GetDefaultName())
            {
                indexBuilder.HasName(index.Name);
            }

            indexBuilder.Metadata.AddAnnotations(index.GetAnnotations());

            return indexBuilder;
        }

        protected virtual ModelBuilder VisitForeignKeys([NotNull] ModelBuilder modelBuilder, [NotNull] IList<DatabaseForeignKey> foreignKeys)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(foreignKeys, nameof(foreignKeys));

            foreach (var fk in foreignKeys)
            {
                this.VisitForeignKey(modelBuilder, fk);
            }

            // Note: must completely assign all foreign keys before assigning
            // navigation properties otherwise naming of navigation properties
            // when there are multiple foreign keys does not work.
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(et => et.GetForeignKeys()))
            {
                this.AddNavigationProperties(foreignKey);
            }

            return modelBuilder;
        }

        protected virtual IMutableForeignKey VisitForeignKey([NotNull] ModelBuilder modelBuilder, [NotNull] DatabaseForeignKey foreignKey)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(foreignKey, nameof(foreignKey));

            if (foreignKey.PrincipalTable == null)
            {
                this._reporter.WriteWarning(
                    DesignStrings.ForeignKeyScaffoldErrorPrincipalTableNotFound(foreignKey.DisplayName()));
                return null;
            }

            if (foreignKey.Table == null)
            {
                return null;
            }

            var dependentEntityType = modelBuilder.Model.FindEntityType(this.GetEntityTypeName(foreignKey.Table));

            if (dependentEntityType == null)
            {
                return null;
            }

            var unmappedDependentColumns = foreignKey.Columns
                .Where(c => this._unmappedColumns.Contains(c))
                .Select(c => c.Name)
                .ToList();
            if (unmappedDependentColumns.Count > 0)
            {
                this._reporter.WriteWarning(
                    DesignStrings.ForeignKeyScaffoldErrorPropertyNotFound(
                        foreignKey.DisplayName(),
                        string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, unmappedDependentColumns)));
                return null;
            }

            var dependentProperties = foreignKey.Columns
                .Select(this.GetPropertyName)
                .Select(name => dependentEntityType.FindProperty(name))
                .ToList()
                .AsReadOnly();

            var principalEntityType = modelBuilder.Model.FindEntityType(this.GetEntityTypeName(foreignKey.PrincipalTable));
            if (principalEntityType == null)
            {
                this._reporter.WriteWarning(
                    DesignStrings.ForeignKeyScaffoldErrorPrincipalTableScaffoldingError(
                        foreignKey.DisplayName(),
                        foreignKey.PrincipalTable.DisplayName()));
                return null;
            }

            var unmappedPrincipalColumns = foreignKey.PrincipalColumns
                .Where(pc => principalEntityType.FindProperty(this.GetPropertyName(pc)) == null)
                .Select(pc => pc.Name)
                .ToList();
            if (unmappedPrincipalColumns.Count > 0)
            {
                this._reporter.WriteWarning(
                    DesignStrings.ForeignKeyScaffoldErrorPropertyNotFound(
                        foreignKey.DisplayName(),
                        string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, unmappedPrincipalColumns)));
                return null;
            }

            var principalPropertiesMap = foreignKey.PrincipalColumns
                .Select(
                    fc => (property: principalEntityType.FindProperty(this.GetPropertyName(fc)), column: fc)).ToList();
            var principalProperties = principalPropertiesMap
                .Select(tuple => tuple.property)
                .ToList();

            var principalKey = principalEntityType.FindKey(principalProperties);
            if (principalKey == null)
            {
                var index = principalEntityType.FindIndex(principalProperties.AsReadOnly());
                if (index?.IsUnique == true)
                {
                    // ensure all principal properties are non-nullable even if the columns
                    // are nullable on the database. EF's concept of a key requires this.
                    var nullablePrincipalProperties =
                        principalPropertiesMap.Where(tuple => tuple.property.IsNullable).ToList();
                    if (nullablePrincipalProperties.Count > 0)
                    {
                        this._reporter.WriteWarning(
                            DesignStrings.ForeignKeyPrincipalEndContainsNullableColumns(
                                foreignKey.DisplayName(),
                                index.GetName(),
                                nullablePrincipalProperties.Select(tuple => tuple.column.DisplayName()).ToList()
                                    .Aggregate((a, b) => a + "," + b)));

                        nullablePrincipalProperties
                            .ToList()
                            .ForEach(tuple => tuple.property.IsNullable = false);
                    }

                    principalKey = principalEntityType.AddKey(principalProperties);
                }
                else
                {
                    var principalColumns = foreignKey.PrincipalColumns.Select(c => c.Name).ToList();

                    this._reporter.WriteWarning(
                        DesignStrings.ForeignKeyScaffoldErrorPrincipalKeyNotFound(
                            foreignKey.DisplayName(),
                            string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, principalColumns),
                            principalEntityType.DisplayName()));

                    return null;
                }
            }

            var newForeignKey = dependentEntityType.AddForeignKey(
                dependentProperties, principalKey, principalEntityType);

            var dependentKey = dependentEntityType.FindKey(dependentProperties);
            var dependentIndex = dependentEntityType.FindIndex(dependentProperties);
            newForeignKey.IsUnique = dependentKey != null
                           || dependentIndex?.IsUnique == true;

            if (!string.IsNullOrEmpty(foreignKey.Name)
                && foreignKey.Name != newForeignKey.GetDefaultName())
            {
                newForeignKey.SetConstraintName(foreignKey.Name);
            }

            AssignOnDeleteAction(foreignKey, newForeignKey);

            newForeignKey.AddAnnotations(foreignKey.GetAnnotations());

            return newForeignKey;
        }

        protected virtual void AddNavigationProperties([NotNull] IMutableForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            var dependentEndExistingIdentifiers = this.ExistingIdentifiers(foreignKey.DeclaringEntityType);
            var dependentEndNavigationPropertyCandidateName =
                this._candidateNamingService.GetDependentEndCandidateNavigationPropertyName(foreignKey);
            var dependentEndNavigationPropertyName =
                this._cSharpUtilities.GenerateCSharpIdentifier(
                    dependentEndNavigationPropertyCandidateName,
                    dependentEndExistingIdentifiers,
                    singularizePluralizer: null,
                    uniquifier: NavigationUniquifier);

            foreignKey.HasDependentToPrincipal(dependentEndNavigationPropertyName);

            var principalEndExistingIdentifiers = this.ExistingIdentifiers(foreignKey.PrincipalEntityType);
            var principalEndNavigationPropertyCandidateName = foreignKey.IsSelfReferencing()
                ? string.Format(
                    CultureInfo.CurrentCulture,
                    SelfReferencingPrincipalEndNavigationNamePattern,
                    dependentEndNavigationPropertyName)
                : this._candidateNamingService.GetPrincipalEndCandidateNavigationPropertyName(
                    foreignKey, dependentEndNavigationPropertyName);

            if (!foreignKey.IsUnique
                && !foreignKey.IsSelfReferencing())
            {
                principalEndNavigationPropertyCandidateName = this._pluralizer.Pluralize(principalEndNavigationPropertyCandidateName);
            }

            var principalEndNavigationPropertyName =
                this._cSharpUtilities.GenerateCSharpIdentifier(
                    principalEndNavigationPropertyCandidateName,
                    principalEndExistingIdentifiers,
                    singularizePluralizer: null,
                    uniquifier: NavigationUniquifier);

            foreignKey.HasPrincipalToDependent(principalEndNavigationPropertyName);
        }

        // Stores the names of the EntityType itself and its Properties, but does not include any Navigation Properties
        private readonly Dictionary<IEntityType, List<string>> _entityTypeAndPropertyIdentifiers = new Dictionary<IEntityType, List<string>>();

        protected virtual List<string> ExistingIdentifiers([NotNull] IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            if (!this._entityTypeAndPropertyIdentifiers.TryGetValue(entityType, out var existingIdentifiers))
            {
                existingIdentifiers = new List<string>
                {
                    entityType.Name
                };
                existingIdentifiers.AddRange(entityType.GetProperties().Select(p => p.Name));
                this._entityTypeAndPropertyIdentifiers[entityType] = existingIdentifiers;
            }

            existingIdentifiers.AddRange(entityType.GetNavigations().Select(p => p.Name));
            return existingIdentifiers;
        }

        protected virtual TypeScaffoldingInfo GetTypeScaffoldingInfo([NotNull] DatabaseColumn column)
        {
            if (column.StoreType == null)
            {
                return null;
            }

            return this._scaffoldingTypeMapper.FindMapping(
                column.StoreType,
                column.IsKeyOrIndex(),
                column.IsRowVersion());
        }

        private static void AssignOnDeleteAction(
            [NotNull] DatabaseForeignKey databaseForeignKey, [NotNull] IMutableForeignKey foreignKey)
        {
            Check.NotNull(databaseForeignKey, nameof(databaseForeignKey));
            Check.NotNull(foreignKey, nameof(foreignKey));

            switch (databaseForeignKey.OnDelete)
            {
                case ReferentialAction.Cascade:
                    foreignKey.DeleteBehavior = DeleteBehavior.Cascade;
                    break;

                case ReferentialAction.SetNull:
                    foreignKey.DeleteBehavior = DeleteBehavior.SetNull;
                    break;

                default:
                    foreignKey.DeleteBehavior = DeleteBehavior.ClientSetNull;
                    break;
            }
        }

        // TODO use CSharpUniqueNamer
        private static string NavigationUniquifier([NotNull] string proposedIdentifier, [CanBeNull] ICollection<string> existingIdentifiers)
        {
            if (existingIdentifiers?.Contains(proposedIdentifier) != true)
            {
                return proposedIdentifier;
            }

            var finalIdentifier =
                string.Format(CultureInfo.CurrentCulture, NavigationNameUniquifyingPattern, proposedIdentifier);
            var suffix = 1;
            while (existingIdentifiers.Contains(finalIdentifier))
            {
                finalIdentifier = proposedIdentifier + suffix;
                suffix++;
            }

            return finalIdentifier;
        }
    }
}
