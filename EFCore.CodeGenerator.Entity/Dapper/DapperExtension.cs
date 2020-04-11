namespace EFCore.CodeGenerator.Entity.Dapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Dapper;
    using Microsoft.Data.SqlClient;

    public static class DapperExtension
    {
        static DapperExtension()
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            SqlMapper.AddTypeHandler(new DateTimeToTicksHandler());
            SqlMapper.AddTypeHandler(new UriToStringHandler());
        }

        public static IDbConnection Connection => new SqlConnection(CodeGenerator.Connection.ConnectionString);

        public static T Find<T>(params object[] keyValues)
        {
            using var connection = Connection;
            var keys = Utilities.PK<T>().Split(',');
            string sql = $"SELECT * FROM {Utilities.TableOrView<T>()} WHERE " + string.Join(" AND ", keys.Select(o => $"{o}=@{o}"));
            var parameters = new DynamicParameters();
            for (int i = 0; i < keyValues.Length; i++)
            {
                parameters.Add(keys[i], keyValues[i]);
            }

            return connection.QueryFirstOrDefault<T>(sql, parameters);
        }

        // TODO: Multi-keys are not supported so far.
        public static int Delete<T>(int id)
        {
            using var connection = Connection;
            PrepareDelete<T>(id, out string sql, out DynamicParameters parameters);
            return connection.Execute(sql, parameters);
        }

        // TODO: Multi-keys are not supported so far.
        public static int Delete<T>(T entity)
        {
            using var connection = Connection;
            PrepareDelete(entity, out string sql, out DynamicParameters parameters);
            return connection.Execute(sql, parameters);
        }

        public static int Insert<T>(T entity)
        {
            using var connection = Connection;
            PrepareInsert(entity, out string sql, out DynamicParameters parameters);
            return connection.Execute(sql, parameters);
        }

        public static int Update<T>(T entity)
        {
            using var connection = Connection;
            PrepareUpdate(entity, out string sql, out DynamicParameters parameters);
            return connection.Execute(sql, parameters);
        }

        public static T FirstOrDefault<T>()
        {
            using var connection = Connection;
            PrepareFirst<T>(out string sql);
            return connection.QueryFirstOrDefault<T>(sql);
        }

        public static async Task<T> FirstOrDefaultAsync<T>()
        {
            using var connection = Connection;
            PrepareFirst<T>(out string sql);
            return await connection.QueryFirstOrDefaultAsync<T>(sql);
        }

        public static async Task<int> DeleteAsync<T>(int id)
        {
            using var connection = Connection;
            PrepareDelete<T>(id, out string sql, out DynamicParameters parameters);
            return await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
        }

        public static IEnumerable<T> FindAll<T>()
        {
            using var connection = Connection;
            string sql = $"SELECT * FROM {Utilities.TableOrView<T>()}";
            return connection.Query<T>(sql);
        }

        public static IEnumerable<T> Page<T>(int size)
        {
            using var connection = Connection;
            string sql = $"SELECT TOP {size} * FROM {Utilities.TableOrView<T>()}";
            return connection.Query<T>(sql);
        }

        internal static T Query<T>(string sql, object param = null)
        {
            using var connection = Connection;
            sql = $"SELECT * FROM {Utilities.TableOrView<T>()} WHERE " + sql;
            return connection.QueryFirst<T>(sql, param);
        }

        internal static void PrepareDelete<T>(int id, out string sql, out DynamicParameters parameters)
        {
            var keys = Utilities.PK<T>();
            sql = $"DELETE FROM {Utilities.TableOrView<T>()} WHERE {keys}=@{keys}";
            parameters = new DynamicParameters();
            parameters.Add(keys, id);
        }

        internal static void PrepareDelete<T>(T entity, out string sql, out DynamicParameters parameters)
        {
            var keys = Utilities.PK<T>();
            sql = $"DELETE FROM {Utilities.TableOrView<T>()} WHERE {keys}=@{keys}";
            parameters = new DynamicParameters();
            string pkPropertyName = Utilities.GetProperties<T>().FirstOrDefault(o => o.ColumnName == keys).Name;
            var pkPropertyInfo = Utilities.GetTypeProperties<T>().FirstOrDefault(o => o.Name == pkPropertyName);
            parameters.Add(keys, pkPropertyInfo.GetValue(entity));
        }

        internal static void PrepareInsert<T>(T entity, out string sql, out DynamicParameters parameters)
        {
            sql = $"INSERT INTO {Utilities.TableOrView<T>()}({string.Join(",", GetColumnList(entity))}) VALUES ({string.Join(",", GetColumnList(entity).Select(column => "@" + column))})";
            PrepareUpdateOrInsertParameters(entity, out parameters);
        }

        internal static void PrepareUpdate<T>(T entity, out string sql, out DynamicParameters parameters)
        {
            sql = $"UPDATE {Utilities.TableOrView<T>()} SET {GetUpdateSetClause(entity)} WHERE {Utilities.PK<T>()}=@{Utilities.PK<T>()}";
            PrepareUpdateOrInsertParameters(entity, out parameters);
        }

        internal static void PrepareUpdateOrInsertParameters<T>(T entity, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();

            // Some parameters may not be needed (automatically generated by the database).
            foreach (var item in Utilities.GetTypeProperties<T>())
            {
                var property = Utilities.DbSetting.GetEntity<T>().Properties.FirstOrDefault(o => o.Name == item.Name);
                if (property != null)
                {
                    parameters.Add(property.ColumnName, ValueConverter.GetConvertedValue(entity, item, property));
                }
            }
        }

        internal static void PrepareFirst<T>(out string sql) => sql = $"SELECT TOP 1 * FROM {Utilities.TableOrView<T>()}";

        internal static string GetUpdateSetClause<T>(T entity) => string.Join(",", GetColumnList(entity).Select(o => $"{o}=@{o}"));

        internal static IList<string> GetColumnList<T>(T entity)
        {
            var values = new List<string>();
            foreach (Property property in Utilities.GetProperties<T>())
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
                    var propertyInfo = Utilities.GetTypeProperties<T>().FirstOrDefault(o => o.Name == property.Name);
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
                            throw new NotSupportedException("Not Supported");
                    }
                }
            }

            return values;
        }
    }
}