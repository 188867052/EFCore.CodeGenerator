using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace EFCore.Scaffolding.Extension.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class Property
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string DefaultValueSql { get; set; }

        [XmlAttribute]
        public string ValueGenerated { get; set; }

        [XmlAttribute]
        public string ColumnName { get; set; }

        [XmlAttribute]
        public string Converter { get; set; }

        [XmlAttribute]
        public string CSharpType { get; set; }

        [XmlAttribute]
        public string Summary { get; set; }

        [XmlIgnore]
        public ConverterEnum ConverterEnum => string.IsNullOrEmpty(this.Converter) ? default : (ConverterEnum)Enum.Parse(typeof(ConverterEnum), this.Converter);
    }
}