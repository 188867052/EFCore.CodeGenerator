namespace ReleaseManage.ControllerHelper.Scaffolding.Models
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "config", IsNullable = false)]
    public class ScaffoldConfig
    {
        [XmlElement("entity")]
        public Entity[] Entities { get; set; }

        [XmlElement("namespace")]
        public Namespace[] Namespaces { get; set; }
    }
}