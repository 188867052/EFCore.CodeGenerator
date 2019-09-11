namespace ReleaseManage.ControllerHelper.Scaffolding.Models
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class Entity
    {
        [XmlElement(ElementName = "property")]
        public Property[] Properties { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string TableName { get; set; }

        [XmlAttribute]
        public string Summary { get; set; }
    }
}