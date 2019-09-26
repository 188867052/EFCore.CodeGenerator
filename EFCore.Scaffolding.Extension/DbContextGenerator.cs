namespace EFCore.Scaffolding.Extension
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Text;
    using EFCore.Scaffolding.Extension.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
    using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
    using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
    using Microsoft.EntityFrameworkCore.SqlServer.Scaffolding.Internal;
    using Microsoft.Extensions.DependencyInjection;

    public class DbContextGenerator
    {
        private readonly string directory;
        private static Dictionary<string, object> _cache = new Dictionary<string, object>();

        internal IList<WriteAllTextModel> WriteAllTextModels { get; set; }

        internal DbContextGenerator(string @namespace, string contextName, string writeCodePath)
        {
            this.WriteAllTextModels = new List<WriteAllTextModel>();
            this.directory = writeCodePath;
            MyDbContextGenerator dbContextGenerator = (MyDbContextGenerator)Services.GetService<ICSharpDbContextGenerator>();
            MyEntityTypeGenerator entityTypeGenerator = (MyEntityTypeGenerator)Services.GetService<ICSharpEntityTypeGenerator>();
            var scaffoldingModelFactory = (MyScaffoldingModelFactory)Services.GetService<IScaffoldingModelFactory>();
            Model model = (Model)scaffoldingModelFactory.Create(DatabaseModel, false);
            var dbContextCode = dbContextGenerator.WriteCode(model, @namespace, contextName, Connection.ConnectionString, false, false);
            this.WriteAllTextModels.Add(new WriteAllTextModel(dbContextCode, Path.Combine(this.directory, $".{contextName}.cs")));

            Helper.FormattingXml(model, DatabaseModel);
            foreach (var entityType in model.GetEntityTypes())
            {
                var entityCode = entityTypeGenerator.WriteCode(entityType, @namespace, false);
                this.WriteAllTextModels.Add(new WriteAllTextModel(entityCode, Path.Combine(this.directory, entityType.Name + ".cs")));
            }

            this.WriteCode(scaffoldingModelFactory.Data, @namespace);
            this.WriteValueGeneratedModel(DatabaseModel, @namespace);
            this.WriteDefaultValueModel(DatabaseModel, @namespace);
            this.WriteValueConverterModel(dbContextGenerator.KeyValuePairs, @namespace);
        }

        public static DatabaseModel DatabaseModel => GetOrAdd(nameof(DatabaseModel), GetDatabaseModel);

        private static DatabaseModel GetDatabaseModel()
        {
            var logger = Services.GetService<IDiagnosticsLogger<DbLoggerCategory.Scaffolding>>();
            var databaseModelFactory = new SqlServerDatabaseModelFactory(logger);
            using (var connection = new SqlConnection(Connection.ConnectionString))
            {
                return databaseModelFactory.Create(connection, new List<string>(), new List<string>());
            }
        }

        private static T GetOrAdd<T>(string key, Func<T> action)
        {
            if (!_cache.ContainsKey(key))
            {
                T v = action();
                _cache.Add(key, v);
                return v;
            }

            return (T)_cache[key];
        }

        private static IServiceCollection Services => GetOrAdd(nameof(Services), GetServices);

        private static IServiceCollection GetServices()
        {
            var servicesCache = new ServiceCollection()
                  .AddEntityFrameworkDesignTimeServices()
                  .AddSingleton<ICSharpDbContextGenerator, MyDbContextGenerator>()
                  .AddSingleton<ICSharpEntityTypeGenerator, MyEntityTypeGenerator>()
                  .AddSingleton<IScaffoldingModelFactory, MyScaffoldingModelFactory>();
            new SqlServerDesignTimeServices().ConfigureDesignTimeServices(servicesCache);

            return servicesCache;
        }

        internal void WriteCode(Dictionary<string, string> dictionary, string @namespace)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine("    using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine("    [System.Diagnostics.CodeAnalysis.SuppressMessage(\"StyleCop.CSharp.LayoutRules\", \"SA1509:Opening braces should not be preceded by blank line\", Justification = \"<挂起>\")]");
            sb.AppendLine("    public static class DatabaseModel");
            sb.AppendLine("    {");
            sb.AppendLine("        public static Dictionary<string, string> Mapping = new Dictionary<string, string>");
            sb.AppendLine("        {");
            foreach (var data in dictionary)
            {
                if (dictionary.First().Key != data.Key && data.Key.Contains("PrimaryKey"))
                {
                    sb.AppendLine();
                }

                sb.AppendLine($"            {{ \"{data.Key}\", \"{data.Value}\" }},");
            }

            sb.AppendLine("        };");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            if (!Directory.Exists(this.directory))
            {
                Directory.CreateDirectory(this.directory);
            }

            this.WriteAllTextModels.Add(new WriteAllTextModel(sb.ToString(), Path.Combine(this.directory, ".DatabaseModel.cs")));
        }

        internal void WriteValueGeneratedModel(DatabaseModel databaseModel, string @namespace)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine("    using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine("    [System.Diagnostics.CodeAnalysis.SuppressMessage(\"StyleCop.CSharp.LayoutRules\", \"SA1509:Opening braces should not be preceded by blank line\", Justification = \"<挂起>\")]");
            sb.AppendLine("    public static class IncreaseModel");
            sb.AppendLine("    {");
            sb.AppendLine("        public static Dictionary<string, string> Mapping = new Dictionary<string, string>");
            sb.AppendLine("        {");
            foreach (var table in databaseModel.Tables)
            {
                foreach (var column in table.Columns)
                {
                    if (column.ValueGenerated.HasValue)
                    {
                        sb.AppendLine($"            {{ \"{table.Name}.{column.Name}\", \"{column.Name}\" }},");
                    }
                }
            }

            sb.AppendLine("        };");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            if (!Directory.Exists(this.directory))
            {
                Directory.CreateDirectory(this.directory);
            }

            this.WriteAllTextModels.Add(new WriteAllTextModel(sb.ToString(), Path.Combine(this.directory, ".ValueGeneratedModel.cs")));
        }

        internal void WriteDefaultValueModel(DatabaseModel databaseModel, string @namespace)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine("    using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine("    [System.Diagnostics.CodeAnalysis.SuppressMessage(\"StyleCop.CSharp.LayoutRules\", \"SA1509:Opening braces should not be preceded by blank line\", Justification = \"<挂起>\")]");
            sb.AppendLine("    public static class DefaultValueSqlModel");
            sb.AppendLine("    {");
            sb.AppendLine("        public static Dictionary<string, string> Mapping = new Dictionary<string, string>");
            sb.AppendLine("        {");
            foreach (var table in databaseModel.Tables)
            {
                foreach (var column in table.Columns)
                {
                    if (!string.IsNullOrEmpty(column.DefaultValueSql))
                    {
                        sb.AppendLine($"            {{ \"{table.Name}.{column.Name}\", \"{column.Name}\" }},");
                    }
                }
            }

            sb.AppendLine("        };");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            if (!Directory.Exists(this.directory))
            {
                Directory.CreateDirectory(this.directory);
            }

            this.WriteAllTextModels.Add(new WriteAllTextModel(sb.ToString(), Path.Combine(this.directory, ".DefaultValueSqlModel.cs")));
        }

        internal void WriteValueConverterModel(Dictionary<string, string> keyValuePairs, string @namespace)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine("    using System.Collections.Generic;");
            sb.AppendLine("    using EFCore.Scaffolding.Extension.Entity.Dapper;");
            sb.AppendLine();
            sb.AppendLine("    [System.Diagnostics.CodeAnalysis.SuppressMessage(\"StyleCop.CSharp.LayoutRules\", \"SA1509:Opening braces should not be preceded by blank line\", Justification = \"<挂起>\")]");
            sb.AppendLine("    public static class ValueConverterModel");
            sb.AppendLine("    {");
            sb.AppendLine("        public static Dictionary<string, ConverterEnum> Mapping = new Dictionary<string, ConverterEnum>");
            sb.AppendLine("        {");
            foreach (var kvp in keyValuePairs)
            {
                sb.AppendLine($"            {{ \"{kvp.Key}\", {kvp.Value} }},");
            }

            sb.AppendLine("        };");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            if (!Directory.Exists(this.directory))
            {
                Directory.CreateDirectory(this.directory);
            }

            this.WriteAllTextModels.Add(new WriteAllTextModel(sb.ToString(), Path.Combine(this.directory, ".ValueConverterModel.cs")));
        }

        internal void WriteTo()
        {
            if (!Directory.Exists(this.directory))
            {
                Directory.CreateDirectory(this.directory);
            }

            foreach (var model in this.WriteAllTextModels)
            {
                File.WriteAllText(model.Path, model.Code, Encoding.UTF8);
            }
        }
    }
}
