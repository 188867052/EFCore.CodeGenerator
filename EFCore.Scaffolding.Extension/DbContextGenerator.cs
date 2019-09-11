namespace EFCore.Scaffolding.Extension
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using System.Data.SqlClient;
    using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
    using Microsoft.EntityFrameworkCore.SqlServer.Scaffolding.Internal;
    using EFCore.Scaffolding.Extension.Models;
    using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

    public class DbContextGenerator
    {
        public const string ConnectionString = "Data Source=47.105.214.235;Initial Catalog=Scaffolding;Persist Security Info=True;User ID=sa;Password=931592457czA";

        private readonly string directory;

        internal IList<WriteAllTextModel> WriteAllTextModels { get; set; }

        internal DbContextGenerator(string @namespace, string contextName, string writeCodePath)
        {
            WriteAllTextModels = new List<WriteAllTextModel>();
            directory = writeCodePath;

            IServiceCollection services = new ServiceCollection()
                .AddEntityFrameworkDesignTimeServices()
                .AddSingleton<ICSharpDbContextGenerator, MyDbContextGenerator>()
                .AddSingleton<ICSharpEntityTypeGenerator, MyEntityTypeGenerator>()
                .AddSingleton<IScaffoldingModelFactory, MyScaffoldingModelFactory>();

            new SqlServerDesignTimeServices().ConfigureDesignTimeServices(services);
            MyDbContextGenerator dbContextGenerator = (MyDbContextGenerator)services.GetService<ICSharpDbContextGenerator>();
            MyEntityTypeGenerator entityTypeGenerator = (MyEntityTypeGenerator)services.GetService<ICSharpEntityTypeGenerator>();
            var scaffoldingModelFactory = (MyScaffoldingModelFactory)services.GetService<IScaffoldingModelFactory>();
            var databaseModel = GetDatabaseModel();
            Model model = (Model)scaffoldingModelFactory.Create(databaseModel, false);
            var dbContextCode = dbContextGenerator.WriteCode(model, @namespace, contextName, ConnectionString, false, false);
            WriteAllTextModels.Add(new WriteAllTextModel(dbContextCode, Path.Combine(directory, contextName + ".cs")));

            Helper.FormattingXml(model, databaseModel);
            foreach (var entityType in model.GetEntityTypes())
            {
                var entityCode = entityTypeGenerator.WriteCode(entityType, @namespace, false);
                WriteAllTextModels.Add(new WriteAllTextModel(entityCode, Path.Combine(directory, entityType.Name + ".cs")));
            }

            WriteCode(scaffoldingModelFactory.Data, @namespace);
        }

        public static DatabaseModel GetDatabaseModel()
        {
            IServiceCollection services = new ServiceCollection()
               .AddEntityFrameworkDesignTimeServices()
               .AddSingleton<ICSharpDbContextGenerator, MyDbContextGenerator>()
               .AddSingleton<ICSharpEntityTypeGenerator, MyEntityTypeGenerator>()
               .AddSingleton<IScaffoldingModelFactory, MyScaffoldingModelFactory>();

            new SqlServerDesignTimeServices().ConfigureDesignTimeServices(services);
            var logger = services.GetService<IDiagnosticsLogger<DbLoggerCategory.Scaffolding>>();
            var databaseModelFactory = new SqlServerDatabaseModelFactory(logger);
            var connection = new SqlConnection(ConnectionString);
            var databaseModel = databaseModelFactory.Create(connection, new List<string>(), new List<string>());

            return databaseModel;
        }

        internal void WriteCode(Dictionary<string, string> dictionary, string @namespace)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine("    using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine("     [System.Diagnostics.CodeAnalysis.SuppressMessage(\"StyleCop.CSharp.LayoutRules\", \"SA1509:Opening braces should not be preceded by blank line\", Justification = \"<挂起>\")]");
            sb.AppendLine($"    public static class MetaData");
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
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            WriteAllTextModels.Add(new WriteAllTextModel(sb.ToString(), Path.Combine(directory, "MetaData.cs")));
        }

        internal void WriteTo()
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            foreach (var model in WriteAllTextModels)
            {
                File.WriteAllText(model.Path, model.Code, Encoding.UTF8);
            }
        }
    }
}
