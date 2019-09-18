using System.Collections.Generic;
using System.Linq;

namespace EFCore.Scaffolding.Extension
{
    public static class ScaffoldingHelper
    {
        public static IEnumerable<string> Scaffolding(string @namespace, string contextName, string writeCodePath)
        {
            DbContextGenerator generator = new DbContextGenerator(@namespace, contextName, writeCodePath);
            generator.WriteTo();

            return generator.WriteAllTextModels.Select(o => o.Code);
        }
    }
}
