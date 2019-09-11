namespace ReleaseManage.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
    using Microsoft.Extensions.DependencyInjection;
    using ReleaseManage.ControllerHelper.Scaffolding.Models;

    internal static class Helper
    {
        static Helper()
        {
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
            file = Directory.GetFiles(di.Parent.Parent.Parent.Parent.FullName, "_Scaffolding.xml", SearchOption.AllDirectories).FirstOrDefault();
            ScaffoldConfig = GetScaffoldConfig();
        }

        private static string file;

        internal static ScaffoldConfig ScaffoldConfig { get; }

        internal static T GetService<T>(this IServiceCollection services)
        {
            return services.BuildServiceProvider().GetRequiredService<T>();
        }

        internal static void FormattingXml(Model model, DatabaseModel databaseModel)
        {
            var entityTypes = model.GetEntityTypes();
            var newConfig = new ScaffoldConfig
            {
                Namespaces = ScaffoldConfig.Namespaces?.OrderBy(o => o.Value).ToArray(),
                Entities = Array.Empty<Entity>(),
            };

            IList<Entity> list = new List<Entity>();

            foreach (var table in databaseModel.Tables.OrderBy(o => o.Name))
            {
                // TODO: may has issue.
                var entityType = entityTypes.FirstOrDefault(o => table.Name.Replace("_", string.Empty).Equals(o.Name, StringComparison.InvariantCultureIgnoreCase));
                var configEntity = ScaffoldConfig.Entities.FirstOrDefault(o => o.Name == entityType.Name);
                Entity entity = new Entity
                {
                    Name = entityType.Name,
                    //TableName = table.Name,
                    Summary = configEntity?.Summary,
                };
                var properties = entityType.GetProperties();

                IList<ControllerHelper.Scaffolding.Models.Property> propertyList = new List<ControllerHelper.Scaffolding.Models.Property>();
                foreach (var column in table.Columns)
                {
                    // TODO: may has issue.
                    var property = properties.FirstOrDefault(o => o.Name.Equals(column.Name.Replace("_", string.Empty), StringComparison.InvariantCultureIgnoreCase));
                    var configProperty = configEntity?.Properties.FirstOrDefault(o => o.Name == property.Name);
                    var p = new ControllerHelper.Scaffolding.Models.Property
                    {
                        Name = property.Name,
                        //ColumnName = field.Name,
                        Summary = configProperty?.Summary,
                        CSharpType = configProperty?.CSharpType,
                        Converter = configProperty?.Converter,
                    };
                    propertyList.Add(p);
                }

                list.Add(entity);
                entity.Properties = propertyList.ToArray();
            }

            newConfig.Entities = list.ToArray();
            string xmlSerialized = Serialize(newConfig);
            File.WriteAllText(file, xmlSerialized, Encoding.UTF8);
        }

        private static ScaffoldConfig GetScaffoldConfig()
        {
            var xml = File.ReadAllText(file, Encoding.UTF8);
            var scaffoldConfig = Deserialize(xml);

            return scaffoldConfig;
        }

        private static ScaffoldConfig Deserialize(string xml)
        {
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer xmldes = new XmlSerializer(typeof(ScaffoldConfig));
                return (ScaffoldConfig)xmldes.Deserialize(sr);
            }
        }

        private static string Serialize<T>(T config)
        {
            using (StringWriter writer = new StringWriter())
            {
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");
                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(writer, config, namespaces);
                return writer.ToString();
            }
        }
    }
}
