﻿namespace EFCore.CodeGenerator
{
    using System;
    using System.Linq;
    using EFCore.CodeGenerator.Entity.Dapper;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

    internal class MyEntityTypeGenerator
        : CSharpEntityTypeGeneratorBase
    {
        public MyEntityTypeGenerator([NotNull] ICSharpHelper cSharpHelper)
            : base(cSharpHelper)
        {
        }

        protected override string GetPropertyType(IProperty property)
        {
            string typeName = base.GetPropertyType(property);
            var propertyImp = (Microsoft.EntityFrameworkCore.Metadata.Internal.Property)property;
            var fieldConfig = Utilities.DbSetting?.Classes?.FirstOrDefault(o => o.Name == propertyImp?.DeclaringType?.Name)?.Properties?.FirstOrDefault(o => o.Name == property.Name);
            if (fieldConfig != null && !string.IsNullOrEmpty(fieldConfig.Type))
            {
                return fieldConfig.Type;
            }

            return typeName;
        }

        protected override void GetSummary(IProperty property)
        {
            var table = Utilities.DbSetting.Classes.FirstOrDefault(o => o.Name == property.DeclaringEntityType.Name);
            if (table != null)
            {
                foreach (var p in table.Properties.Where(p => !string.IsNullOrEmpty(p.Summary) && p.Name == property.Name))
                {
                    this.IndentedStringBuilder.AppendLine($"/// <summary>");
                    this.IndentedStringBuilder.AppendLine($"/// {p.Summary}.");
                    this.IndentedStringBuilder.AppendLine($"/// </summary>");
                }
            }
        }

        protected override void GetSummary(IEntityType entityType)
        {
            var table = Utilities.DbSetting.Classes.FirstOrDefault(o => o.Name == entityType.Name);
            if (table != null && !string.IsNullOrEmpty(table.Summary))
            {
                this.IndentedStringBuilder.AppendLine($"/// <summary>");
                this.IndentedStringBuilder.AppendLine($"/// {table.Summary}.");
                this.IndentedStringBuilder.AppendLine($"/// </summary>");
            }
        }

        protected override void GenerateNameSpace(IEntityType entityType)
        {
            var table = Utilities.DbSetting.Classes?.FirstOrDefault(o => o.Name == entityType.Name);
            if (table != null)
            {
                foreach (var property in table.Properties.Select(property => property))
                {
                    var ns = Utilities.DbSetting.Namespaces.FirstOrDefault(o => o.Type == property.Type);
                    if (ns != null)
                    {
                        string us = $"using {ns.Value};";
                        if (!this.IndentedStringBuilder.ToString().Contains(us, StringComparison.InvariantCulture))
                        {
                            this.IndentedStringBuilder.AppendLine(us);
                        }
                    }
                }
            }
        }
    }
}
