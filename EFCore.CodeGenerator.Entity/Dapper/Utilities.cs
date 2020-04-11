namespace EFCore.CodeGenerator.Entity.Dapper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml.Serialization;

    public static class Utilities
    {
        private static DbSetting dbSetting;

        public static string DbSettingFile => Directory.GetFiles(DirectoryInfo.FullName, ".dbSetting.xml", SearchOption.AllDirectories).First();

        public static DirectoryInfo DirectoryInfo => new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.Parent;

        internal static string PK<T>() => DbSetting.GetEntity<T>().PrimaryKey;

        internal static string TableOrView<T>() => DbSetting.GetEntity<T>().View ?? DbSetting.GetEntity<T>().TableName;

        internal static Property[] GetProperties<T>() => DbSetting.GetEntity<T>().Properties;

        internal static DbSetting DbSetting
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
    }
}