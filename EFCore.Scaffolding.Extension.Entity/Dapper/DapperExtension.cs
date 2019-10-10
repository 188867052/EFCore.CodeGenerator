namespace EFCore.Scaffolding.Extension.Entity.Dapper
{
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

        public static IDbConnection Connection => new SqlConnection(Extension.Connection.ConnectionString);

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
            Utilities.PrepareDelete<T>(id, out string sql, out DynamicParameters parameters);
            return connection.Execute(sql, parameters);
        }

        // TODO: Multi-keys are not supported so far.
        public static int Delete<T>(T entity)
        {
            using var connection = Connection;
            Utilities.PrepareDelete(entity, out string sql, out DynamicParameters parameters);
            return connection.Execute(sql, parameters);
        }

        public static int Insert<T>(T entity)
        {
            using var connection = Connection;
            Utilities.PrepareInsert(entity, out string sql, out DynamicParameters parameters);
            return connection.Execute(sql, parameters);
        }

        public static int Update<T>(T entity)
        {
            using var connection = Connection;
            Utilities.PrepareUpdate(entity, out string sql, out DynamicParameters parameters);
            return connection.Execute(sql, parameters);
        }

        public static T FirstOrDefault<T>()
        {
            using var connection = Connection;
            Utilities.PrepareFirst<T>(out string sql);
            return connection.QueryFirstOrDefault<T>(sql);
        }

        public static async Task<T> FirstOrDefaultAsync<T>()
        {
            using var connection = Connection;
            Utilities.PrepareFirst<T>(out string sql);
            return await connection.QueryFirstOrDefaultAsync<T>(sql);
        }

        public static async Task<int> DeleteAsync<T>(int id)
        {
            using var connection = Connection;
            Utilities.PrepareDelete<T>(id, out string sql, out DynamicParameters parameters);
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
    }
}