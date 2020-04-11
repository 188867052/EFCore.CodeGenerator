using System.IO;
using System.Linq;
using Dapper;
using EFCore.CodeGenerator.Entity.Dapper;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Xunit;

namespace EFCore.CodeGenerator
{
    public static class Program
    {
        public static void Main()
        {
            RunDatabaseScript();
            var generator = new DbContextGenerator("Entities", "TestDbContext", new FileInfo(Utilities.DbSettingFile).DirectoryName);
            generator.WriteTo();
            DatabaseCheckAllTableHasPK();
        }

        private static void RunDatabaseScript()
        {
            var sqlFile = Utilities.DirectoryInfo.GetFiles(".sql", SearchOption.AllDirectories).FirstOrDefault();
            string sql = File.ReadAllText(sqlFile.FullName);
            if (!string.IsNullOrEmpty(sql))
            {
                var count = DapperExtension.Connection.Execute(sql);
                Assert.Equal(-1, count);
            }
        }

        private static void DatabaseCheckAllTableHasPK()
        {
            foreach (var table in DbContextGenerator.DatabaseModel.Tables)
            {
                if (table.GetType() == typeof(DatabaseTable))
                {
                    Assert.NotEmpty(table.PrimaryKey.Columns);
                }
            }
        }
    }
}
