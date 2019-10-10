namespace EFCore.Scaffolding.Extension
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EFCore.Scaffolding.Extension.Models;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Scaffolding;
    using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

    internal class MyDbContextGenerator : CSharpDbContextGeneratorBase
    {
        [Obsolete]
        public MyDbContextGenerator(
            IProviderConfigurationCodeGenerator providerCodeGenerators,
            IAnnotationCodeGenerator annotationCodeGenerator,
            ICSharpHelper cSharpHelper)
            : base(
                providerCodeGenerators,
                annotationCodeGenerator,
                cSharpHelper)
        {
        }

        protected override void GenerateProperty(IProperty property, bool useDataAnnotations)
        {
            base.GenerateProperty(property, useDataAnnotations);
        }

        protected override void GenerateNameSpace()
        {
            foreach (var property in Helper.ScaffoldConfig.Classes.SelectMany(table => table.Properties.Select(property => property)))
            {
                Namespace ns = Helper.ScaffoldConfig.Namespaces.FirstOrDefault(o => o.Type == property.Type);
                if (ns != null)
                {
                    string us = $"using {ns.Value};";
                    if (!this.sb.ToString().Contains(us, StringComparison.InvariantCulture))
                    {
                        this.sb.AppendLine(us);
                    }
                }
            }

            this.sb.AppendLine("using Microsoft.EntityFrameworkCore.Storage.ValueConversion;");
        }

        protected override List<string> Lines(IProperty property)
        {
            var line = base.Lines(property);
            var propertyImp = (Microsoft.EntityFrameworkCore.Metadata.Internal.Property)property;
            var fieldConfig = Helper.ScaffoldConfig?.Classes?.FirstOrDefault(o => o.Name == propertyImp?.DeclaringType?.Name)?.Properties?.FirstOrDefault(o => o.Name == property.Name);
            if (fieldConfig != null)
            {
                switch (fieldConfig.ConverterEnum)
                {
                    case ValueConverterEnum.DateTimeToTicks:
                        line.Add($@".HasConversion(new DateTimeToTicksConverter())");
                        break;
                    case ValueConverterEnum.EnumToString:
                        line.Add($@".HasConversion(new EnumToStringConverter<{fieldConfig.Type}>())");
                        break;
                    case ValueConverterEnum.BoolToString:
                        line.Add($@".HasConversion(new BoolToStringConverter(bool.FalseString, bool.TrueString))");
                        break;
                    case ValueConverterEnum.BoolToZeroOne:
                        line.Add($@".HasConversion(new BoolToZeroOneConverter<int>())");
                        break;
                    case ValueConverterEnum.UriToString:
                        line.Add($@".HasConversion(new UriToStringConverter())");
                        break;
                    case ValueConverterEnum.None:
                        break;
                    default:
                        throw new ArgumentException($"Converter {fieldConfig.Converter} not exist.");
                }
            }

            return line;
        }
    }
}
