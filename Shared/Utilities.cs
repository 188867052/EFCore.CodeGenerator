using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.CodeGenerator.Entity.Dapper
{
    public static class Utilities
    {
        private static DbSetting dbSetting;

        public static string DbSettingFile => Directory.GetFiles(DirectoryInfo.FullName, ".dbSetting.xml", SearchOption.AllDirectories).First();

        public static DirectoryInfo DirectoryInfo => new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.Parent;

        internal static string PK<T>() => DbSetting.GetEntity<T>().PrimaryKey;

        internal static string TableOrView<T>() => DbSetting.GetEntity<T>().View ?? DbSetting.GetEntity<T>().TableName;

        internal static Property[] GetProperties<T>() => DbSetting.GetEntity<T>().Properties;

        public static DbSetting DbSetting
        {
            get
            {
                if (dbSetting == null)
                {
                    dbSetting = Deserialize(File.ReadAllText(DbSettingFile, Encoding.UTF8));
                }

                return dbSetting;
            }
        }

        internal static List<PropertyInfo> GetTypeProperties<T>() => typeof(T).GetProperties().ToList();

        internal static DbSetting Deserialize(string xml)
        {
            using var sr = new StringReader(xml);
            return (DbSetting)new XmlSerializer(typeof(DbSetting)).Deserialize(sr);
        }

        public static string Serialize<T>(T config)
        {
            using StringWriter writer = new StringWriter();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");
            XmlSerializer xs = new XmlSerializer(typeof(T));
            xs.Serialize(writer, config, namespaces);
            return writer.ToString();
        }

        internal static T GetService<T>(this IServiceCollection services)
        {
            return services.BuildServiceProvider().GetRequiredService<T>();
        }

        internal static void FormattingXml(Model model, DatabaseModel databaseModel)
        {
            var entityTypes = model.GetEntityTypes();
            var newConfig = new DbSetting
            {
                Namespaces = DbSetting.Namespaces?.OrderBy(o => o.Value).ToArray(),
                Classes = Array.Empty<Class>(),
            };

            var list = new List<Class>();

            foreach (var table in databaseModel.Tables.OrderBy(o => o.Name))
            {
                // TODO: may has issue.
                var entityType = entityTypes.FirstOrDefault(o => table.Name.Replace("_", string.Empty).Equals(o.Name, StringComparison.InvariantCultureIgnoreCase));
                var configEntity = DbSetting.Classes.FirstOrDefault(o => o.Name == entityType.Name);
                var entity = new Class
                {
                    Name = entityType.Name,
                    Table = table.GetType() == typeof(DatabaseView) ? null : table.Name,
                    View = table.GetType() == typeof(DatabaseView) ? table.Name : null,
                    Summary = string.IsNullOrEmpty(table.Comment) ? configEntity?.Summary : table.Comment,
                    PrimaryKey = table.PrimaryKey == null ? null : string.Join(",", table.PrimaryKey.Columns.Select(o => o.Name)),
                };

                if (entity.Name == entity.TableName)
                {
                    entity.Table = null;
                }
                var properties = entityType.GetProperties();

                var propertyList = new List<CodeGenerator.Property>();
                foreach (var column in table.Columns)
                {
                    var property = properties.FirstOrDefault(o => o.Name.Equals(column.Name.Replace("_", string.Empty), StringComparison.InvariantCultureIgnoreCase));
                    var configProperty = configEntity?.Properties.FirstOrDefault(o => o.Name == property.Name);
                    string fk = "";

                    // 数据库配置
                    if (column.Table.ForeignKeys.SelectMany(o => o.Columns).Contains(column))
                    {
                        foreach (var item in table.ForeignKeys)
                        {
                            if (item.Columns.Contains(column))
                            {
                                fk = $"{item.PrincipalTable.Name}.{item.PrincipalColumns[0].Name}";
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(fk))
                    {
                        fk = configProperty.FK;
                    }

                    var p = new CodeGenerator.Property
                    {
                        Name = property.Name,
                        DefaultValueSql = column.DefaultValueSql,
                        Column = column.Name,
                        ValueGenerated = column.ValueGenerated?.ToString(),
                        Summary = string.IsNullOrEmpty(column.Comment) ? configProperty?.Summary : column.Comment,
                        Type = configProperty?.Type,
                        Converter = configProperty?.Converter,
                        FK = fk,
                    };

                    if (p.Name == p.ColumnName)
                    {
                        p.Column = null;
                    }

                    propertyList.Add(p);
                }

                list.Add(entity);
                entity.Properties = propertyList.ToArray();
            }

            newConfig.Classes = list.ToArray();
            string xmlSerialized = Serialize(newConfig);
            File.WriteAllText(DbSettingFile, xmlSerialized, Encoding.UTF8);
        }
    }
}