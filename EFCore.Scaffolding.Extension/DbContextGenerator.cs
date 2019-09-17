namespace EFCore.Scaffolding.Extension
{
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
        public const string ConnectionString = "Data Source=47.105.214.235;Initial Catalog=Scaffolding;Persist Security Info=True;User ID=sa;Password=931592457czA";
        private readonly string directory;
        private static DatabaseModel databaseModel;

        internal IList<WriteAllTextModel> WriteAllTextModels { get; set; }

        internal DbContextGenerator(string @namespace, string contextName, string writeCodePath)
        {
            this.WriteAllTextModels = new List<WriteAllTextModel>();
            this.directory = writeCodePath;

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
            this.WriteAllTextModels.Add(new WriteAllTextModel(dbContextCode, Path.Combine(this.directory, contextName + ".cs")));

            Helper.FormattingXml(model, databaseModel);
            foreach (var entityType in model.GetEntityTypes())
            {
                var entityCode = entityTypeGenerator.WriteCode(entityType, @namespace, false);
                this.WriteAllTextModels.Add(new WriteAllTextModel(entityCode, Path.Combine(this.directory, entityType.Name + ".cs")));
            }

            this.WriteCode(scaffoldingModelFactory.Data, @namespace);
        }

        public static DatabaseModel GetDatabaseModel()
        {
            if (databaseModel == null)
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
                databaseModel = databaseModelFactory.Create(connection, new List<string>(), new List<string>());
            }

            return databaseModel;
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
