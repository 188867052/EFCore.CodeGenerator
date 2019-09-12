﻿namespace EFCore.Scaffolding.Extension
{
    using System;
    using System.Linq;
    using EFCore.Scaffolding.Extension.Models;
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
            var fieldConfig = Helper.ScaffoldConfig?.Entities?.FirstOrDefault(o => o.Name == propertyImp?.DeclaringType?.Name)?.Properties?.FirstOrDefault(o => o.Name == property.Name);
            if (fieldConfig != null && !string.IsNullOrEmpty(fieldConfig.CSharpType))
            {
                return fieldConfig.CSharpType;
            }

            return typeName;
        }

        protected override void GetSummary(IProperty property)
        {
            var table = Helper.ScaffoldConfig.Entities.FirstOrDefault(o => o.Name == property.DeclaringEntityType.Name);
            if (table != null)
            {
                foreach (var p in table.Properties.Where(p => !string.IsNullOrEmpty(p.Summary) && p.Name == property.Name))
                {
                    IndentedStringBuilder.AppendLine($"/// <summary>");
                    IndentedStringBuilder.AppendLine($"/// {p.Summary}.");
                    IndentedStringBuilder.AppendLine($"/// </summary>");
                }
            }
        }

        protected override void GetSummary(IEntityType entityType)
        {
            var table = Helper.ScaffoldConfig.Entities.FirstOrDefault(o => o.Name == entityType.Name);
            if (table != null && !string.IsNullOrEmpty(table.Summary))
            {
                IndentedStringBuilder.AppendLine($"/// <summary>");
                IndentedStringBuilder.AppendLine($"/// {table.Summary}.");
                IndentedStringBuilder.AppendLine($"/// </summary>");
            }
        }

        protected override void GenerateNameSpace(IEntityType entityType)
        {
            var table = Helper.ScaffoldConfig.Entities.FirstOrDefault(o => o.Name == entityType.Name);
            if (table != null)
            {
                foreach (Property property in table.Properties.Where(property => !string.IsNullOrEmpty(property.Converter)).Select(property => property))
                {
                    Namespace ns = Helper.ScaffoldConfig.Namespaces.FirstOrDefault(o => o.Name == property.CSharpType);
                    if (ns != default)
                    {
                        string us = $"using {ns.Value};";
                        if (!IndentedStringBuilder.ToString().Contains(us, StringComparison.InvariantCulture))
                        {
                            IndentedStringBuilder.AppendLine(us);
                        }
                    }
                }
            }
        }
    }
}