namespace EFCore.Scaffolding.Extension.Models
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class Namespace
    {
        [XmlAttribute]
        public string Type { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}