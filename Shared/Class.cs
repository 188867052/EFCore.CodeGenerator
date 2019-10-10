namespace EFCore.Scaffolding.Extension.Models
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class Class
    {
        [XmlElement(ElementName = "property")]
        public Property[] Properties { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Table { get; set; }

        [XmlAttribute]
        public string View { get; set; }

        [XmlAttribute]
        public string Summary { get; set; }

        [XmlAttribute]
        public string PrimaryKey { get; set; }
    }
}