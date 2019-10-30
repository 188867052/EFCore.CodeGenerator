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
    using Microsoft.EntityFrameworkCore.Scaffolding;
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
            var scaffoldingModelFactory = (RelationalScaffoldingModelFactory)Services.GetService<IScaffoldingModelFactory>();
            Model model = (Model)scaffoldingModelFactory.Create(DatabaseModel, false);
            var dbContextCode = dbContextGenerator.WriteCode(model, @namespace, contextName, Connection.ConnectionString, false, false);
            this.WriteAllTextModels.Add(new WriteAllTextModel(dbContextCode, Path.Combine(this.directory, $".{contextName}.cs")));

            Helper.FormattingXml(model, DatabaseModel);
            foreach (var entityType in model.GetEntityTypes())
            {
                var entityCode = entityTypeGenerator.WriteCode(entityType, @namespace, false);
                this.WriteAllTextModels.Add(new WriteAllTextModel(entityCode, Path.Combine(this.directory, entityType.Name + ".cs")));
            }
        }

        public static DatabaseModel DatabaseModel => GetOrAdd(nameof(DatabaseModel), GetDatabaseModel);

        private static DatabaseModel GetDatabaseModel()
        {
            var logger = Services.GetService<IDiagnosticsLogger<DbLoggerCategory.Scaffolding>>();
            var databaseModelFactory = new SqlServerDatabaseModelFactory(logger);
            using var connection = new SqlConnection(Connection.ConnectionString);
            return databaseModelFactory.Create(connection, new DatabaseModelFactoryOptions(new List<string>(), new List<string>()));
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
                  .AddSingleton<ICSharpEntityTypeGenerator, MyEntityTypeGenerator>();
            new SqlServerDesignTimeServices().ConfigureDesignTimeServices(servicesCache);

            return servicesCache;
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
