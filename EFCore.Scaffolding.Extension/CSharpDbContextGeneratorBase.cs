namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EFCore.Scaffolding.Extension;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    public abstract class CSharpDbContextGeneratorBase : ICSharpDbContextGenerator
    {
        private const string EntityLambdaIdentifier = "entity";
        private const string Language = "CSharp";

        private readonly ICSharpHelper _code;
        private readonly IScaffoldingProviderCodeGenerator _legacyProviderCodeGenerator;
        private readonly IProviderConfigurationCodeGenerator _providerConfigurationCodeGenerator;
        private readonly IAnnotationCodeGenerator _annotationCodeGenerator;
        protected IndentedStringBuilder sb;
        private bool _entityTypeBuilderInitialized;

        public CSharpDbContextGeneratorBase(
            [NotNull] IEnumerable<IScaffoldingProviderCodeGenerator> legacyProviderCodeGenerators,
            [NotNull] IEnumerable<IProviderConfigurationCodeGenerator> providerCodeGenerators,
            [NotNull] IAnnotationCodeGenerator annotationCodeGenerator,
            [NotNull] ICSharpHelper cSharpHelper)
        {
            Check.NotNull(legacyProviderCodeGenerators, nameof(legacyProviderCodeGenerators));
            Check.NotNull(providerCodeGenerators, nameof(providerCodeGenerators));
            Check.NotNull(annotationCodeGenerator, nameof(annotationCodeGenerator));
            Check.NotNull(cSharpHelper, nameof(cSharpHelper));

            if (!legacyProviderCodeGenerators.Any() && !providerCodeGenerators.Any())
            {
                throw new ArgumentException(AbstractionsStrings.CollectionArgumentIsEmpty(nameof(providerCodeGenerators)));
            }

            this._legacyProviderCodeGenerator = legacyProviderCodeGenerators.LastOrDefault();
            this._providerConfigurationCodeGenerator = providerCodeGenerators.LastOrDefault();
            this._annotationCodeGenerator = annotationCodeGenerator;
            this._code = cSharpHelper;
        }

        protected virtual void GenerateNameSpace()
        {
        }

        public virtual string WriteCode(IModel model, string @namespace, string contextName, string connectionString, bool useDataAnnotations, bool suppressConnectionStringWarning)
        {
            Check.NotNull(model, nameof(model));

            this.sb = new IndentedStringBuilder();

            this.sb.AppendLine("using System;"); // Guid default values require new Guid() which requires this using
            this.sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            this.sb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata;");
            this.GenerateNameSpace();
            this.sb.AppendLine();

            this.sb.AppendLine($"namespace {@namespace}");
            this.sb.AppendLine("{");

            using (this.sb.Indent())
            {
                this.GenerateClass(
                    model,
                    contextName,
                    connectionString,
                    useDataAnnotations,
                    suppressConnectionStringWarning);
            }

            this.sb.AppendLine("}");

            return this.sb.ToString();
        }

        protected virtual void GenerateClass(
            [NotNull] IModel model,
            [NotNull] string contextName,
            [NotNull] string connectionString,
            bool useDataAnnotations,
            bool suppressConnectionStringWarning)
        {
            Check.NotNull(model, nameof(model));
            Check.NotNull(contextName, nameof(contextName));
            Check.NotNull(connectionString, nameof(connectionString));

            this.sb.AppendLine($"public partial class {contextName} : DbContext");
            this.sb.AppendLine("{");

            using (this.sb.Indent())
            {
                this.GenerateConstructors(contextName);
                this.GenerateDbSets(model);
                this.GenerateEntityTypeErrors(model);
                this.GenerateOnConfiguring(connectionString, suppressConnectionStringWarning);
                this.GenerateOnModelCreating(model, useDataAnnotations);
            }

            this.sb.AppendLine("}");
        }

        private void GenerateConstructors(string contextName)
        {
            this.sb.AppendLine($"public {contextName}()")
                .AppendLine("{")
                .AppendLine("}")
                .AppendLine();

            this.sb.AppendLine($"public {contextName}(DbContextOptions<{contextName}> options)")
                .IncrementIndent()
                .AppendLine(": base(options)")
                .DecrementIndent()
                .AppendLine("{")
                .AppendLine("}")
                .AppendLine();
        }

        private void GenerateDbSets(IModel model)
        {
            var entityTypes = model.GetEntityTypes();
            foreach (var entityType in entityTypes)
            {
                this.sb.AppendLine($"public virtual DbSet<{entityType.Name}> {entityType.Scaffolding().DbSetName} {{ get; set; }}");
                if (entityTypes.IndexOf(entityType) < entityTypes.Count() - 1)
                {
                    this.sb.AppendLine();
                }
            }

            if (model.GetEntityTypes().Any())
            {
                this.sb.AppendLine();
            }
        }

        private void GenerateEntityTypeErrors(IModel model)
        {
            foreach (var entityTypeError in model.Scaffolding().EntityTypeErrors)
            {
                this.sb.AppendLine($"// {entityTypeError.Value} Please see the warning messages.");
            }

            if (model.Scaffolding().EntityTypeErrors.Any())
            {
                this.sb.AppendLine();
            }
        }

        protected virtual void GenerateOnConfiguring(
            [NotNull] string connectionString,
            bool suppressConnectionStringWarning)
        {
            Check.NotNull(connectionString, nameof(connectionString));

            this.sb.AppendLine("protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)");
            this.sb.AppendLine("{");

            using (this.sb.Indent())
            {
                this.sb.AppendLine("if (!optionsBuilder.IsConfigured)");
                this.sb.AppendLine("{");

                using (this.sb.Indent())
                {
                    this.sb.AppendLine($"optionsBuilder.UseSqlServer(\"{DbContextGenerator.ConnectionString}\");");
                }

                this.sb.AppendLine("}");
            }

            this.sb.AppendLine("}");
            this.sb.AppendLine();
        }

        protected virtual void GenerateOnModelCreating(
            [NotNull] IModel model,
            bool useDataAnnotations)
        {
            Check.NotNull(model, nameof(model));

            this.sb.AppendLine("protected override void OnModelCreating(ModelBuilder modelBuilder)");
            this.sb.Append("{");

            var annotations = model.GetAnnotations().ToList();
            RemoveAnnotation(ref annotations, ChangeDetector.SkipDetectChangesAnnotation);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.MaxIdentifierLength);
            RemoveAnnotation(ref annotations, ScaffoldingAnnotationNames.DatabaseName);
            RemoveAnnotation(ref annotations, ScaffoldingAnnotationNames.EntityTypeErrors);

            var annotationsToRemove = new List<IAnnotation>();
            annotationsToRemove.AddRange(
                annotations.Where(
                    a => a.Name.StartsWith(RelationalAnnotationNames.SequencePrefix, StringComparison.Ordinal)));

            var lines = new List<string>();

            foreach (var annotation in annotations)
            {
                if (annotation.Value == null
                    || this._annotationCodeGenerator.IsHandledByConvention(model, annotation))
                {
                    annotationsToRemove.Add(annotation);
                }
                else
                {
                    var methodCall = this._annotationCodeGenerator.GenerateFluentApi(model, annotation);
                    var line = methodCall == null
                        ? this._annotationCodeGenerator.GenerateFluentApi(model, annotation, Language)
                        : this._code.Fragment(methodCall);

                    if (line != null)
                    {
                        lines.Add(line);
                        annotationsToRemove.Add(annotation);
                    }
                }
            }

            lines.AddRange(this.GenerateAnnotations(annotations.Except(annotationsToRemove)));

            if (lines.Count > 0)
            {
                using (this.sb.Indent())
                {
                    this.sb.AppendLine();
                    this.sb.Append("modelBuilder" + lines[0]);

                    using (this.sb.Indent())
                    {
                        foreach (var line in lines.Skip(1))
                        {
                            this.sb.AppendLine();
                            this.sb.Append(line);
                        }
                    }

                    this.sb.AppendLine(";");
                }
            }

            using (this.sb.Indent())
            {
                foreach (var entityType in model.GetEntityTypes())
                {
                    this._entityTypeBuilderInitialized = false;

                    this.GenerateEntityType(entityType, useDataAnnotations);

                    if (this._entityTypeBuilderInitialized)
                    {
                        this.sb.AppendLine("});");
                    }
                }

                foreach (var sequence in model.Relational().Sequences)
                {
                    this.GenerateSequence(sequence);
                }
            }

            this.sb.AppendLine("}");
        }

        private void InitializeEntityTypeBuilder(IEntityType entityType)
        {
            if (!this._entityTypeBuilderInitialized)
            {
                this.sb.AppendLine();
                this.sb.AppendLine($"modelBuilder.Entity<{entityType.Name}>({EntityLambdaIdentifier} =>");
                this.sb.Append("{");
            }

            this._entityTypeBuilderInitialized = true;
        }

        private void GenerateEntityType(IEntityType entityType, bool useDataAnnotations)
        {
            this.GenerateKey(entityType.FindPrimaryKey(), useDataAnnotations);

            var annotations = entityType.GetAnnotations().ToList();
            RemoveAnnotation(ref annotations, CoreAnnotationNames.ConstructorBinding);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.TableName);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.Schema);
            RemoveAnnotation(ref annotations, ScaffoldingAnnotationNames.DbSetName);

            if (!useDataAnnotations)
            {
                this.GenerateTableName(entityType);
            }

            var annotationsToRemove = new List<IAnnotation>();
            var lines = new List<string>();

            foreach (var annotation in annotations)
            {
                if (this._annotationCodeGenerator.IsHandledByConvention(entityType, annotation))
                {
                    annotationsToRemove.Add(annotation);
                }
                else
                {
                    var methodCall = this._annotationCodeGenerator.GenerateFluentApi(entityType, annotation);
                    var line = methodCall == null
                        ? this._annotationCodeGenerator.GenerateFluentApi(entityType, annotation, Language)
                        : this._code.Fragment(methodCall);

                    if (line != null)
                    {
                        lines.Add(line);
                        annotationsToRemove.Add(annotation);
                    }
                }
            }

            lines.AddRange(this.GenerateAnnotations(annotations.Except(annotationsToRemove)));

            this.AppendMultiLineFluentApi(entityType, lines);

            foreach (var index in entityType.GetIndexes())
            {
                this.GenerateIndex(index);
            }

            foreach (var property in entityType.GetProperties())
            {
                this.GenerateProperty(property, useDataAnnotations);
            }

            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                this.GenerateRelationship(foreignKey, useDataAnnotations);
            }
        }

        private void AppendMultiLineFluentApi(IEntityType entityType, IList<string> lines)
        {
            if (lines.Count <= 0)
            {
                return;
            }

            this.InitializeEntityTypeBuilder(entityType);

            using (this.sb.Indent())
            {
                this.sb.AppendLine();

                this.sb.Append(EntityLambdaIdentifier + lines[0]);

                using (this.sb.Indent())
                {
                    foreach (var line in lines.Skip(1))
                    {
                        this.sb.AppendLine();
                        this.sb.Append(line);
                    }
                }

                this.sb.AppendLine(";");
            }
        }

        private void GenerateKey(IKey key, bool useDataAnnotations)
        {
            if (key == null)
            {
                return;
            }

            var annotations = key.GetAnnotations().ToList();

            var explicitName = key.Relational().Name != ConstraintNamer.GetDefaultName(key);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.Name);

            if (key.Properties.Count == 1
                && annotations.Count == 0)
            {
                if (key is Key concreteKey
                    && key.Properties.SequenceEqual(
                        new KeyDiscoveryConvention(null).DiscoverKeyProperties(
                            concreteKey.DeclaringEntityType,
                            concreteKey.DeclaringEntityType.GetProperties().ToList())))
                {
                    return;
                }

                if (!explicitName
                    && useDataAnnotations)
                {
                    return;
                }
            }

            var lines = new List<string>
            {
                $".{nameof(EntityTypeBuilder.HasKey)}(e => {GenerateLambdaToKey(key.Properties, "e")})",
            };

            if (explicitName)
            {
                lines.Add($".{nameof(RelationalKeyBuilderExtensions.HasName)}" +
                    $"({this._code.Literal(key.Relational().Name)})");
            }

            var annotationsToRemove = new List<IAnnotation>();

            foreach (var annotation in annotations)
            {
                if (this._annotationCodeGenerator.IsHandledByConvention(key, annotation))
                {
                    annotationsToRemove.Add(annotation);
                }
                else
                {
                    var methodCall = this._annotationCodeGenerator.GenerateFluentApi(key, annotation);
                    var line = methodCall == null
#pragma warning disable CS0618 // Type or member is obsolete
                        ? this._annotationCodeGenerator.GenerateFluentApi(key, annotation, Language)
#pragma warning restore CS0618 // Type or member is obsolete
                        : this._code.Fragment(methodCall);

                    if (line != null)
                    {
                        lines.Add(line);
                        annotationsToRemove.Add(annotation);
                    }
                }
            }

            lines.AddRange(this.GenerateAnnotations(annotations.Except(annotationsToRemove)));

            this.AppendMultiLineFluentApi(key.DeclaringEntityType, lines);
        }

        private void GenerateTableName(IEntityType entityType)
        {
            var tableName = entityType.Relational().TableName;
            var schema = entityType.Relational().Schema;
            var defaultSchema = entityType.Model.Relational().DefaultSchema;

            var explicitSchema = schema != null && schema != defaultSchema;
            var explicitTable = explicitSchema || tableName != null && tableName != entityType.Scaffolding().DbSetName;

            if (explicitTable)
            {
                var parameterString = this._code.Literal(tableName);
                if (explicitSchema)
                {
                    parameterString += ", " + this._code.Literal(schema);
                }

                var lines = new List<string>
                {
                    $".{nameof(RelationalEntityTypeBuilderExtensions.ToTable)}({parameterString})"
                };

                this.AppendMultiLineFluentApi(entityType, lines);
            }
        }

        private void GenerateIndex(IIndex index)
        {
            var lines = new List<string>
            {
                $".{nameof(EntityTypeBuilder.HasIndex)}(e => {GenerateLambdaToKey(index.Properties, "e")})"
            };

            var annotations = index.GetAnnotations().ToList();

            if (!string.IsNullOrEmpty((string)index[RelationalAnnotationNames.Name]))
            {
                lines.Add(
                    $".{nameof(RelationalIndexBuilderExtensions.HasName)}" +
                    $"({this._code.Literal(index.Relational().Name)})");
                RemoveAnnotation(ref annotations, RelationalAnnotationNames.Name);
            }

            if (index.IsUnique)
            {
                lines.Add($".{nameof(IndexBuilder.IsUnique)}()");
            }

            if (index.Relational().Filter != null)
            {
                lines.Add(
                    $".{nameof(RelationalIndexBuilderExtensions.HasFilter)}" +
                    $"({this._code.Literal(index.Relational().Filter)})");
                RemoveAnnotation(ref annotations, RelationalAnnotationNames.Filter);
            }

            var annotationsToRemove = new List<IAnnotation>();

            foreach (var annotation in annotations)
            {
                if (this._annotationCodeGenerator.IsHandledByConvention(index, annotation))
                {
                    annotationsToRemove.Add(annotation);
                }
                else
                {
                    var methodCall = this._annotationCodeGenerator.GenerateFluentApi(index, annotation);
                    var line = methodCall == null
#pragma warning disable CS0618 // Type or member is obsolete
                        ? this._annotationCodeGenerator.GenerateFluentApi(index, annotation, Language)
#pragma warning restore CS0618 // Type or member is obsolete
                        : this._code.Fragment(methodCall);

                    if (line != null)
                    {
                        lines.Add(line);
                        annotationsToRemove.Add(annotation);
                    }
                }
            }

            lines.AddRange(this.GenerateAnnotations(annotations.Except(annotationsToRemove)));

            this.AppendMultiLineFluentApi(index.DeclaringEntityType, lines);
        }

        protected virtual List<string> Lines(IProperty property)
        {
            return new List<string>
            {
                $".{nameof(EntityTypeBuilder.Property)}(e => e.{property.Name})",
            };
        }

        protected virtual void GenerateProperty(IProperty property, bool useDataAnnotations)
        {
            var lines = this.Lines(property);

            var annotations = property.GetAnnotations().ToList();

            RemoveAnnotation(ref annotations, RelationalAnnotationNames.ColumnName);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.ColumnType);
            RemoveAnnotation(ref annotations, CoreAnnotationNames.MaxLengthAnnotation);
            RemoveAnnotation(ref annotations, CoreAnnotationNames.UnicodeAnnotation);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.DefaultValue);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.DefaultValueSql);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.ComputedColumnSql);
            RemoveAnnotation(ref annotations, RelationalAnnotationNames.IsFixedLength);
            RemoveAnnotation(ref annotations, ScaffoldingAnnotationNames.ColumnOrdinal);

            if (!useDataAnnotations)
            {
                if (!property.IsNullable
                    && property.ClrType.IsNullableType()
                    && !property.IsPrimaryKey())
                {
                    lines.Add($".{nameof(PropertyBuilder.IsRequired)}()");
                }

                var columnName = property.Relational().ColumnName;

                if (columnName != null
                    && columnName != property.Name)
                {
                    lines.Add(
                        $".{nameof(RelationalPropertyBuilderExtensions.HasColumnName)}" +
                        $"({this._code.Literal(columnName)})");
                }

                var columnType = property.GetConfiguredColumnType();

                if (columnType != null)
                {
                    lines.Add(
                        $".{nameof(RelationalPropertyBuilderExtensions.HasColumnType)}" +
                        $"({this._code.Literal(columnType)})");
                }

                var maxLength = property.GetMaxLength();

                if (maxLength.HasValue)
                {
                    lines.Add(
                        $".{nameof(PropertyBuilder.HasMaxLength)}" +
                        $"({this._code.Literal(maxLength.Value)})");
                }
            }

            if (property.IsUnicode() != null)
            {
                lines.Add(
                    $".{nameof(PropertyBuilder.IsUnicode)}" +
                    $"({(property.IsUnicode() == false ? "false" : "")})");
            }

            if (property.Relational().IsFixedLength)
            {
                lines.Add(
                    $".{nameof(RelationalPropertyBuilderExtensions.IsFixedLength)}()");
            }

            if (property.Relational().DefaultValue != null)
            {
                lines.Add(
                    $".{nameof(RelationalPropertyBuilderExtensions.HasDefaultValue)}" +
                    $"({this._code.UnknownLiteral(property.Relational().DefaultValue)})");
            }

            if (property.Relational().DefaultValueSql != null)
            {
                lines.Add(
                    $".{nameof(RelationalPropertyBuilderExtensions.HasDefaultValueSql)}" +
                    $"({this._code.Literal(property.Relational().DefaultValueSql)})");
            }

            if (property.Relational().ComputedColumnSql != null)
            {
                lines.Add(
                    $".{nameof(RelationalPropertyBuilderExtensions.HasComputedColumnSql)}" +
                    $"({this._code.Literal(property.Relational().ComputedColumnSql)})");
            }

            var valueGenerated = property.ValueGenerated;
            var isRowVersion = false;
            if (((Property)property).GetValueGeneratedConfigurationSource().HasValue
                && new RelationalValueGeneratorConvention().GetValueGenerated((Property)property) != valueGenerated)
            {
                string methodName;
                switch (valueGenerated)
                {
                    case ValueGenerated.OnAdd:
                        methodName = nameof(PropertyBuilder.ValueGeneratedOnAdd);
                        break;

                    case ValueGenerated.OnAddOrUpdate:
                        isRowVersion = property.IsConcurrencyToken;
                        methodName = isRowVersion
                            ? nameof(PropertyBuilder.IsRowVersion)
                            : nameof(PropertyBuilder.ValueGeneratedOnAddOrUpdate);
                        break;

                    case ValueGenerated.Never:
                        methodName = nameof(PropertyBuilder.ValueGeneratedNever);
                        break;

                    default:
                        methodName = "";
                        break;
                }

                lines.Add($".{methodName}()");
            }

            if (property.IsConcurrencyToken
                && !isRowVersion)
            {
                lines.Add($".{nameof(PropertyBuilder.IsConcurrencyToken)}()");
            }

            var annotationsToRemove = new List<IAnnotation>();

            foreach (var annotation in annotations)
            {
                if (this._annotationCodeGenerator.IsHandledByConvention(property, annotation))
                {
                    annotationsToRemove.Add(annotation);
                }
                else
                {
                    var methodCall = this._annotationCodeGenerator.GenerateFluentApi(property, annotation);
                    var line = methodCall == null
#pragma warning disable CS0618 // Type or member is obsolete
                        ? this._annotationCodeGenerator.GenerateFluentApi(property, annotation, Language)
#pragma warning restore CS0618 // Type or member is obsolete
                        : this._code.Fragment(methodCall);

                    if (line != null)
                    {
                        lines.Add(line);
                        annotationsToRemove.Add(annotation);
                    }
                }
            }

            lines.AddRange(this.GenerateAnnotations(annotations.Except(annotationsToRemove)));

            switch (lines.Count)
            {
                case 1:
                    return;
                case 2:
                    lines = new List<string>
                    {
                        lines[0] + lines[1]
                    };
                    break;
            }

            this.AppendMultiLineFluentApi(property.DeclaringEntityType, lines);
        }

        private void GenerateRelationship(IForeignKey foreignKey, bool useDataAnnotations)
        {
            var canUseDataAnnotations = true;
            var annotations = foreignKey.GetAnnotations().ToList();

            var lines = new List<string>
            {
                $".{nameof(EntityTypeBuilder.HasOne)}(d => d.{foreignKey.DependentToPrincipal.Name})",
                $".{(foreignKey.IsUnique ? nameof(ReferenceNavigationBuilder.WithOne) : nameof(ReferenceNavigationBuilder.WithMany))}"
                + $"(p => p.{foreignKey.PrincipalToDependent.Name})"
            };

            if (!foreignKey.PrincipalKey.IsPrimaryKey())
            {
                canUseDataAnnotations = false;
                lines.Add(
                    $".{nameof(ReferenceReferenceBuilder.HasPrincipalKey)}"
                    + $"{(foreignKey.IsUnique ? $"<{foreignKey.PrincipalEntityType.DisplayName()}>" : "")}"
                    + $"(p => {GenerateLambdaToKey(foreignKey.PrincipalKey.Properties, "p")})");
            }

            lines.Add(
                $".{nameof(ReferenceReferenceBuilder.HasForeignKey)}"
                + $"{(foreignKey.IsUnique ? $"<{foreignKey.DeclaringEntityType.DisplayName()}>" : "")}"
                + $"(d => {GenerateLambdaToKey(foreignKey.Properties, "d")})");

            var defaultOnDeleteAction = foreignKey.IsRequired
                ? DeleteBehavior.Cascade
                : DeleteBehavior.ClientSetNull;

            if (foreignKey.DeleteBehavior != defaultOnDeleteAction)
            {
                canUseDataAnnotations = false;
                lines.Add(
                    $".{nameof(ReferenceReferenceBuilder.OnDelete)}" +
                    $"({this._code.Literal(foreignKey.DeleteBehavior)})");
            }

            if (!string.IsNullOrEmpty((string)foreignKey[RelationalAnnotationNames.Name]))
            {
                canUseDataAnnotations = false;
                lines.Add(
                    $".{nameof(RelationalReferenceReferenceBuilderExtensions.HasConstraintName)}" +
                    $"({this._code.Literal(foreignKey.Relational().Name)})");
                RemoveAnnotation(ref annotations, RelationalAnnotationNames.Name);
            }

            var annotationsToRemove = new List<IAnnotation>();

            foreach (var annotation in annotations)
            {
                if (this._annotationCodeGenerator.IsHandledByConvention(foreignKey, annotation))
                {
                    annotationsToRemove.Add(annotation);
                }
                else
                {
                    var methodCall = this._annotationCodeGenerator.GenerateFluentApi(foreignKey, annotation);
                    var line = methodCall == null
#pragma warning disable CS0618 // Type or member is obsolete
                        ? this._annotationCodeGenerator.GenerateFluentApi(foreignKey, annotation, Language)
#pragma warning restore CS0618 // Type or member is obsolete
                        : this._code.Fragment(methodCall);

                    if (line != null)
                    {
                        canUseDataAnnotations = false;
                        lines.Add(line);
                        annotationsToRemove.Add(annotation);
                    }
                }
            }

            lines.AddRange(this.GenerateAnnotations(annotations.Except(annotationsToRemove)));

            if (!useDataAnnotations
                || !canUseDataAnnotations)
            {
                this.AppendMultiLineFluentApi(foreignKey.DeclaringEntityType, lines);
            }
        }

        private void GenerateSequence(ISequence sequence)
        {
            var methodName = nameof(RelationalModelBuilderExtensions.HasSequence);

            if (sequence.ClrType != Sequence.DefaultClrType)
            {
                methodName += $"<{this._code.Reference(sequence.ClrType)}>";
            }

            var parameters = this._code.Literal(sequence.Name);

            if (string.IsNullOrEmpty(sequence.Schema)
                && sequence.Model.Relational().DefaultSchema != sequence.Schema)
            {
                parameters += $", {this._code.Literal(sequence.Schema)}";
            }

            var lines = new List<string>
            {
                $"modelBuilder.{methodName}({parameters})"
            };

            if (sequence.StartValue != Sequence.DefaultStartValue)
            {
                lines.Add($".{nameof(SequenceBuilder.StartsAt)}({sequence.StartValue})");
            }

            if (sequence.IncrementBy != Sequence.DefaultIncrementBy)
            {
                lines.Add($".{nameof(SequenceBuilder.IncrementsBy)}({sequence.IncrementBy})");
            }

            if (sequence.MinValue != Sequence.DefaultMinValue)
            {
                lines.Add($".{nameof(SequenceBuilder.HasMin)}({sequence.MinValue})");
            }

            if (sequence.MaxValue != Sequence.DefaultMaxValue)
            {
                lines.Add($".{nameof(SequenceBuilder.HasMax)}({sequence.MaxValue})");
            }

            if (sequence.IsCyclic != Sequence.DefaultIsCyclic)
            {
                lines.Add($".{nameof(SequenceBuilder.IsCyclic)}()");
            }

            if (lines.Count == 2)
            {
                lines = new List<string>
                {
                    lines[0] + lines[1]
                };
            }

            this.sb.AppendLine();
            this.sb.Append(lines[0]);

            using (this.sb.Indent())
            {
                foreach (var line in lines.Skip(1))
                {
                    this.sb.AppendLine();
                    this.sb.Append(line);
                }
            }

            this.sb.AppendLine(";");
        }

        private static string GenerateLambdaToKey(
            IReadOnlyList<IProperty> properties,
            string lambdaIdentifier)
        {
            if (properties.Count <= 0)
            {
                return "";
            }

            return properties.Count == 1
                ? $"{lambdaIdentifier}.{properties[0].Name}"
                : $"new {{ {string.Join(", ", properties.Select(p => lambdaIdentifier + "." + p.Name))} }}";
        }

        private static void RemoveAnnotation(ref List<IAnnotation> annotations, string annotationName)
            => annotations.Remove(annotations.SingleOrDefault(a => a.Name == annotationName));

        private IList<string> GenerateAnnotations(IEnumerable<IAnnotation> annotations)
            => annotations.Select(this.GenerateAnnotation).ToList();

        private string GenerateAnnotation(IAnnotation annotation)
            => $".HasAnnotation({this._code.Literal(annotation.Name)}, " +
            $"{this._code.UnknownLiteral(annotation.Value)})";
    }
}
