namespace EFCore.Scaffolding.Extension.Entity.Dapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using EFCore.Scaffolding.Extension.Models;
    using global::Dapper;

    public static class DapperHelper
    {
        private static readonly ScaffoldConfig ScaffoldConfig;
        private static readonly string file;

        static DapperHelper()
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            SqlMapper.AddTypeHandler(new DateTimeToTicksHandler());
            SqlMapper.AddTypeHandler(new UriToStringHandler());
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
            file = Directory.GetFiles(di.Parent.Parent.Parent.Parent.FullName, ".Scaffolding.xml", SearchOption.AllDirectories).FirstOrDefault();
            ScaffoldConfig = GetScaffoldConfig();
        }

        public static IDbConnection Connection => new SqlConnection(Extension.Connection.ConnectionString);

        public static T Find<T>(params object[] keyValues)
        {
            using (Connection)
            {
                var keys = PK<T>().Split(',');
                string sql = $"SELECT * FROM {TableName<T>()} WHERE " + string.Join(" AND ", keys.Select(o => $"{o}=@{o}"));
                var parameters = new DynamicParameters();
                for (int i = 0; i < keyValues.Length; i++)
                {
                    parameters.Add(keys[i], keyValues[i]);
                }

                return Connection.QueryFirstOrDefault<T>(sql, parameters);
            }
        }

        // TODO: Multi-keys are not supported so far.
        public static int Delete<T>(int id)
        {
            using (Connection)
            {
                PrepareDelete<T>(id, out string sql, out DynamicParameters parameters);
                return Connection.Execute(sql, parameters);
            }
        }

        // TODO: Multi-keys are not supported so far.
        public static int Delete<T>(T entity)
        {
            using (Connection)
            {
                PrepareDelete(entity, out string sql, out DynamicParameters parameters);
                return Connection.Execute(sql, parameters);
            }
        }

        public static int Insert<T>(T entity)
        {
            using (Connection)
            {
                PrepareInsert(entity, out string sql, out DynamicParameters parameters);
                return Connection.Execute(sql, parameters);
            }
        }

        public static int Update<T>(T entity)
        {
            using (Connection)
            {
                PrepareUpdate(entity, out string sql, out DynamicParameters parameters);
                return Connection.Execute(sql, parameters);
            }
        }

        public static T FirstOrDefault<T>()
        {
            using (Connection)
            {
                PrepareFirst<T>(out string sql);
                return Connection.QueryFirstOrDefault<T>(sql);
            }
        }

        public static async Task<int> DeleteAsync<T>(int id)
        {
            using (Connection)
            {
                PrepareDelete<T>(id, out string sql, out DynamicParameters parameters);
                return await Connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
            }
        }

        public static IEnumerable<T> FindAll<T>()
        {
            using (Connection)
            {
                string sql = $"SELECT * FROM {TableName<T>()}";
                return Connection.Query<T>(sql);
            }
        }

        public static IEnumerable<T> Page<T>(int size)
        {
            using (Connection)
            {
                string sql = $"SELECT TOP {size} * FROM {TableName<T>()}";
                return Connection.Query<T>(sql);
            }
        }

        internal static T Query<T>(string sql, object param = null)
        {
            sql = $"SELECT * FROM {TableName<T>()} WHERE " + sql;
            return Connection.QueryFirst<T>(sql, param);
        }

        private static void PrepareDelete<T>(int id, out string sql, out DynamicParameters parameters)
        {
            var keys = PK<T>();
            sql = $"DELETE FROM {TableName<T>()} WHERE {keys}=@{keys}";
            parameters = new DynamicParameters();
            parameters.Add(keys, id);
        }

        private static void PrepareDelete<T>(T entity, out string sql, out DynamicParameters parameters)
        {
            var keys = PK<T>();
            sql = $"DELETE FROM {TableName<T>()} WHERE {keys}=@{keys}";
            parameters = new DynamicParameters();
            string pkPropertyName = GetProperties<T>().FirstOrDefault(o => o.ColumnName == keys).Name;
            var pkPropertyInfo = GetPropertyInfos<T>().FirstOrDefault(o => o.Name == pkPropertyName);
            var pkValue = pkPropertyInfo.GetValue(entity);
            parameters.Add(keys, pkValue);
        }

        private static void PrepareInsert<T>(T entity, out string sql, out DynamicParameters parameters)
        {
            sql = $"INSERT INTO {TableName<T>()}({string.Join(",", GetColumnList(entity))}) VALUES ({string.Join(",", GetColumnList(entity).Select(column => "@" + column))})";
            PrepareUpdateOrInsertParameters(entity, out parameters);
        }

        private static void PrepareUpdate<T>(T entity, out string sql, out DynamicParameters parameters)
        {
            sql = $"UPDATE {TableName<T>()} SET {GetUpdateSetClause(entity)} WHERE {PK<T>()}=@{PK<T>()}";
            PrepareUpdateOrInsertParameters(entity, out parameters);
        }

        private static void PrepareUpdateOrInsertParameters<T>(T entity, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();

            // Some parameters may not be needed (automatically generated by the database).
            foreach (var item in GetPropertyInfos<T>())
            {
                var property = ScaffoldConfig.GetEntity<T>().Properties.FirstOrDefault(o => o.Name == item.Name);
                if (property != null)
                {
                    parameters.Add(property.ColumnName, ValueConverter.GetConvertedValue(entity, item, property));
                }
            }
        }

        private static void PrepareFirst<T>(out string sql)
        {
            sql = $"SELECT TOP 1 * FROM {TableName<T>()}";
        }

        private static string GetUpdateSetClause<T>(T entity)
        {
            return string.Join(",", GetColumnList(entity).Select(o => $"{o}=@{o}"));
        }

        private static IList<string> GetColumnList<T>(T entity)
        {
            List<string> values = new List<string>();
            foreach (Property property in GetProperties<T>())
            {
                bool isIncrease = !string.IsNullOrEmpty(property.ValueGenerated);
                bool isDefaultValueSql = !string.IsNullOrEmpty(property.DefaultValueSql);

                // Not self-incrementing, no default value (usually a non-primary key field)
                if (!isIncrease && !isDefaultValueSql)
                {
                    values.Add(property.ColumnName);
                    continue;
                }

                // When the space is empty, the database automatically takes the default value field and the value of the property is not the default value.
                if (isDefaultValueSql)
                {
                    var propertyInfo = GetPropertyInfos<T>().FirstOrDefault(o => o.Name == property.Name);
                    var value = propertyInfo.GetValue(entity);
                    switch (propertyInfo.PropertyType.Name)
                    {
                        case nameof(Guid):
                            if ((Guid)value != default)
                            {
                                values.Add(property.ColumnName);
                            }

                            continue;
                        case nameof(Int32):
                            if ((int)value != default)
                            {
                                values.Add(property.ColumnName);
                            }

                            continue;
                        default:
                            throw new Exception("Exceptions");
                    }
                }
            }

            return values;
        }

        private static string PK<T>()
        {
            return ScaffoldConfig.GetEntity<T>().PrimaryKey;
        }

        private static string TableName<T>()
        {
            return ScaffoldConfig.GetEntity<T>().TableName;
        }

        private static Property[] GetProperties<T>()
        {
            return ScaffoldConfig.GetEntity<T>().Properties;
        }

        private static ScaffoldConfig GetScaffoldConfig()
        {
            var xml = File.ReadAllText(file, Encoding.UTF8);
            var scaffoldConfig = Deserialize(xml);

            return scaffoldConfig;
        }

        private static List<PropertyInfo> GetPropertyInfos<T>()
        {
            return typeof(T).GetProperties().ToList();
        }

        private static ScaffoldConfig Deserialize(string xml)
        {
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer xmldes = new XmlSerializer(typeof(ScaffoldConfig));
                return (ScaffoldConfig)xmldes.Deserialize(sr);
            }
        }
    }
}