namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using EFCore.Scaffolding.Extension;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Design.Internal;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    public abstract class CSharpEntityTypeGeneratorBase : ICSharpEntityTypeGenerator
    {
        private readonly ICSharpHelper code;
        private bool useDataAnnotations;

        protected IndentedStringBuilder IndentedStringBuilder { get; set; }

        public CSharpEntityTypeGeneratorBase(
            [NotNull] ICSharpHelper cSharpHelper)
        {
            Check.NotNull(cSharpHelper, nameof(cSharpHelper));

            this.code = cSharpHelper;
        }

        protected virtual void GenerateNameSpace(IEntityType entityType)
        {
        }

        public virtual string WriteCode(IEntityType entityType, string @namespace, bool useDataAnnotations)
        {
            Check.NotNull(entityType, nameof(entityType));
            Check.NotNull(@namespace, nameof(@namespace));

            this.IndentedStringBuilder = new IndentedStringBuilder();
            this.useDataAnnotations = useDataAnnotations;

            this.IndentedStringBuilder.AppendLine("using System;");
            this.IndentedStringBuilder.AppendLine("using System.Collections.Generic;");
            this.GenerateNameSpace(entityType);

            if (this.useDataAnnotations)
            {
                this.IndentedStringBuilder.AppendLine("using System.ComponentModel.DataAnnotations;");
                this.IndentedStringBuilder.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            }

            foreach (var ns in entityType.GetProperties()
                .SelectMany(p => p.ClrType.GetNamespaces())
                .Where(ns => ns != "System" && ns != "System.Collections.Generic")
                .Distinct()
                .OrderBy(x => x, new NamespaceComparer()))
            {
                this.IndentedStringBuilder.AppendLine($"using {ns};");
            }

            this.IndentedStringBuilder.AppendLine();
            this.IndentedStringBuilder.AppendLine($"namespace {@namespace}");
            this.IndentedStringBuilder.AppendLine("{");

            using (this.IndentedStringBuilder.Indent())
            {
                this.GenerateClass(entityType);
            }

            this.IndentedStringBuilder.AppendLine("}");

            return this.IndentedStringBuilder.ToString();
        }

        protected virtual void GenerateClass(
            [NotNull] IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            if (this.useDataAnnotations)
            {
                this.GenerateEntityTypeDataAnnotations(entityType);
            }

            this.GetSummary(entityType);
            this.IndentedStringBuilder.AppendLine($"public partial class {entityType.Name}");

            this.IndentedStringBuilder.AppendLine("{");

            using (this.IndentedStringBuilder.Indent())
            {
                this.GenerateConstructor(entityType);
                this.GenerateProperties(entityType);
                this.GenerateNavigationProperties(entityType);
            }

            this.IndentedStringBuilder.AppendLine("}");
        }

        protected virtual void GenerateEntityTypeDataAnnotations(
            [NotNull] IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            this.GenerateTableAttribute(entityType);
        }

        private void GenerateTableAttribute(IEntityType entityType)
        {
            var tableName = entityType.Relational().TableName;
            var schema = entityType.Relational().Schema;
            var defaultSchema = entityType.Model.Relational().DefaultSchema;

            var schemaParameterNeeded = schema != null && schema != defaultSchema;
            var tableAttributeNeeded = schemaParameterNeeded || tableName != null && tableName != entityType.Scaffolding().DbSetName;

            if (tableAttributeNeeded)
            {
                var tableAttribute = new AttributeWriter(nameof(TableAttribute));

                tableAttribute.AddParameter(this.code.Literal(tableName));

                if (schemaParameterNeeded)
                {
                    tableAttribute.AddParameter($"{nameof(TableAttribute.Schema)} = {this.code.Literal(schema)}");
                }

                this.IndentedStringBuilder.AppendLine(tableAttribute.ToString());
            }
        }

        protected virtual void GenerateConstructor(
            [NotNull] IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var collectionNavigations = entityType.GetNavigations().Where(n => n.IsCollection()).ToList();

            if (collectionNavigations.Count > 0)
            {
                this.IndentedStringBuilder.AppendLine($"public {entityType.Name}()");
                this.IndentedStringBuilder.AppendLine("{");

                using (this.IndentedStringBuilder.Indent())
                {
                    foreach (var navigation in collectionNavigations)
                    {
                        this.IndentedStringBuilder.AppendLine($"this.{navigation.Name} = new HashSet<{navigation.GetTargetType().Name}>();");
                    }
                }

                this.IndentedStringBuilder.AppendLine("}");
                this.IndentedStringBuilder.AppendLine();
            }
        }

        protected virtual void GenerateProperties(
            [NotNull] IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var properties = entityType.GetProperties().OrderBy(p => p.Scaffolding().ColumnOrdinal);
            foreach (var property in properties)
            {
                if (this.useDataAnnotations)
                {
                    this.GeneratePropertyDataAnnotations(property);
                }

                this.GetSummary(property);
                this.IndentedStringBuilder.AppendLine($"public {this.GetPropertyType(property)} {property.Name} {{ get; set; }}");
                if (properties.IndexOf(property) < properties.Count() - 1)
                {
                    this.IndentedStringBuilder.AppendLine();
                }
            }
        }

        protected virtual string GetPropertyType(IProperty property)
        {
            return this.code.Reference(property.ClrType);
        }

        protected virtual void GetSummary(IEntityType entityType)
        {
        }

        protected virtual void GetSummary(IProperty property)
        {
        }

        protected virtual void GeneratePropertyDataAnnotations(
            [NotNull] IProperty property)
        {
            Check.NotNull(property, nameof(property));

            this.GenerateKeyAttribute(property);
            this.GenerateRequiredAttribute(property);
            this.GenerateColumnAttribute(property);
            this.GenerateMaxLengthAttribute(property);
        }

        private void GenerateKeyAttribute(IProperty property)
        {
            var key = property.AsProperty().PrimaryKey;

            if (key?.Properties.Count == 1)
            {
                if (key is Key concreteKey
                    && key.Properties.SequenceEqual(new KeyDiscoveryConvention(null).DiscoverKeyProperties(concreteKey.DeclaringEntityType, concreteKey.DeclaringEntityType.GetProperties().ToList())))
                {
                    return;
                }

                if (key.Relational().Name != ConstraintNamer.GetDefaultName(key))
                {
                    return;
                }

                this.IndentedStringBuilder.AppendLine(new AttributeWriter(nameof(KeyAttribute)));
            }
        }

        private void GenerateColumnAttribute(IProperty property)
        {
            var columnName = property.Relational().ColumnName;
            var columnType = property.GetConfiguredColumnType();

            var delimitedColumnName = columnName != null && columnName != property.Name ? this.code.Literal(columnName) : null;
            var delimitedColumnType = columnType != null ? this.code.Literal(columnType) : null;

            if ((delimitedColumnName ?? delimitedColumnType) != null)
            {
                var columnAttribute = new AttributeWriter(nameof(ColumnAttribute));

                if (delimitedColumnName != null)
                {
                    columnAttribute.AddParameter(delimitedColumnName);
                }

                if (delimitedColumnType != null)
                {
                    columnAttribute.AddParameter($"{nameof(ColumnAttribute.TypeName)} = {delimitedColumnType}");
                }

                this.IndentedStringBuilder.AppendLine(columnAttribute);
            }
        }

        private void GenerateMaxLengthAttribute(IProperty property)
        {
            var maxLength = property.GetMaxLength();

            if (maxLength.HasValue)
            {
                var lengthAttribute = new AttributeWriter(
                    property.ClrType == typeof(string)
                        ? nameof(StringLengthAttribute)
                        : nameof(MaxLengthAttribute));

                lengthAttribute.AddParameter(this.code.Literal(maxLength.Value));

                this.IndentedStringBuilder.AppendLine(lengthAttribute.ToString());
            }
        }

        private void GenerateRequiredAttribute(IProperty property)
        {
            if (!property.IsNullable
                && property.ClrType.IsNullableType()
                && !property.IsPrimaryKey())
            {
                this.IndentedStringBuilder.AppendLine(new AttributeWriter(nameof(RequiredAttribute)).ToString());
            }
        }

        protected virtual void GenerateNavigationProperties(
            [NotNull] IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            var sortedNavigations = entityType.GetNavigations()
                .OrderBy(n => n.IsDependentToPrincipal() ? 0 : 1)
                .ThenBy(n => n.IsCollection() ? 1 : 0);

            if (sortedNavigations.Any())
            {
                this.IndentedStringBuilder.AppendLine();

                foreach (var navigation in sortedNavigations)
                {
                    if (this.useDataAnnotations)
                    {
                        this.GenerateNavigationDataAnnotations(navigation);
                    }

                    var referencedTypeName = navigation.GetTargetType().Name;
                    var navigationType = navigation.IsCollection() ? $"ICollection<{referencedTypeName}>" : referencedTypeName;
                    this.IndentedStringBuilder.AppendLine($"public {navigationType} {navigation.Name} {{ get; set; }}");

                    if (sortedNavigations.IndexOf(navigation) < sortedNavigations.Count() - 1)
                    {
                        this.IndentedStringBuilder.AppendLine();
                    }
                }
            }
        }

        private void GenerateNavigationDataAnnotations(INavigation navigation)
        {
            this.GenerateForeignKeyAttribute(navigation);
            this.GenerateInversePropertyAttribute(navigation);
        }

        private void GenerateForeignKeyAttribute(INavigation navigation)
        {
            if (navigation.IsDependentToPrincipal())
            {
                if (navigation.ForeignKey.PrincipalKey.IsPrimaryKey())
                {
                    var foreignKeyAttribute = new AttributeWriter(nameof(ForeignKeyAttribute));

                    foreignKeyAttribute.AddParameter(
                        this.code.Literal(
                            string.Join(",", navigation.ForeignKey.Properties.Select(p => p.Name))));

                    this.IndentedStringBuilder.AppendLine(foreignKeyAttribute.ToString());
                }
            }
        }

        private void GenerateInversePropertyAttribute(INavigation navigation)
        {
            if (navigation.ForeignKey.PrincipalKey.IsPrimaryKey())
            {
                var inverseNavigation = navigation.FindInverse();

                if (inverseNavigation != null)
                {
                    var inversePropertyAttribute = new AttributeWriter(nameof(InversePropertyAttribute));

                    inversePropertyAttribute.AddParameter(this.code.Literal(inverseNavigation.Name));

                    this.IndentedStringBuilder.AppendLine(inversePropertyAttribute.ToString());
                }
            }
        }

        private class AttributeWriter
        {
            private readonly string attibuteName;
            private readonly List<string> parameters = new List<string>();

            public AttributeWriter([NotNull] string attributeName)
            {
                Check.NotEmpty(attributeName, nameof(attributeName));

                this.attibuteName = attributeName;
            }

            public void AddParameter([NotNull] string parameter)
            {
                Check.NotEmpty(parameter, nameof(parameter));

                this.parameters.Add(parameter);
            }

            public override string ToString()
                => "[" + (this.parameters.Count == 0
                       ? StripAttribute(this.attibuteName)
                       : StripAttribute(this.attibuteName) + "(" + string.Join(", ", this.parameters) + ")") + "]";

            private static string StripAttribute([NotNull] string attributeName)
                => attributeName.EndsWith("Attribute", StringComparison.Ordinal)
                    ? attributeName.Substring(0, attributeName.Length - 9)
                    : attributeName;
        }
    }
}
