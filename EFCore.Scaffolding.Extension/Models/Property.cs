namespace EFCore.Scaffolding.Extension.Models
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class Property
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string ColumnName { get; set; }

        [XmlAttribute]
        public string Converter { get; set; }

        [XmlAttribute]
        public string CSharpType { get; set; }

        [XmlAttribute]
        public string Summary { get; set; }
    }
}