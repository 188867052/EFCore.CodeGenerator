namespace ReleaseManage.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Scaffolding;
    using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
    using ReleaseManage.ControllerHelper.Scaffolding.Models;

    internal class MyDbContextGenerator : CSharpDbContextGeneratorBase
    {
        [Obsolete]
        public MyDbContextGenerator(
            IEnumerable<IScaffoldingProviderCodeGenerator> legacyProviderCodeGenerators,
            IEnumerable<IProviderConfigurationCodeGenerator> providerCodeGenerators,
            IAnnotationCodeGenerator annotationCodeGenerator,
            ICSharpHelper cSharpHelper)
            : base(
                legacyProviderCodeGenerators,
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
            foreach (var property in Helper.ScaffoldConfig.Entities.SelectMany(table => table.Properties.Where(property => !string.IsNullOrEmpty(property.Converter)).Select(property => property)))
            {
                Namespace ns = Helper.ScaffoldConfig.Namespaces.FirstOrDefault(o => o.Name == property.CSharpType);
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
            var fieldConfig = Helper.ScaffoldConfig?.Entities?.FirstOrDefault(o => o.Name == propertyImp?.DeclaringType?.Name)?.Properties?.FirstOrDefault(o => o.Name == property.Name);
            switch (fieldConfig?.Converter)
            {
                case "DateTimeToTicks":
                    line.Add($@".HasConversion(new DateTimeToTicksConverter())");

                    break;
                case "EnumToString":
                    line.Add($@".HasConversion(new EnumToStringConverter<{fieldConfig.CSharpType}>())");
                    break;
                case "BoolToString":
                    line.Add($@".HasConversion(new BoolToStringConverter(bool.FalseString, bool.TrueString))");
                    break;
            }

            return line;
        }
    }
}
