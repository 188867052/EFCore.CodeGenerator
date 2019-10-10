namespace EFCore.Scaffolding.Extension.Models
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "config", IsNullable = false)]
    public class ScaffoldConfig
    {
        [XmlElement("class")]
        public Class[] Classes { get; set; }

        [XmlElement("namespace")]
        public Namespace[] Namespaces { get; set; }

        public Class GetEntity<T>()
        {
            return this.Classes.FirstOrDefault(o => o.Name == typeof(T).Name);
        }
    }
}