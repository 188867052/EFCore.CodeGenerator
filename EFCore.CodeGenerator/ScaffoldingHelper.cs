using System.Collections.Generic;
using System.Linq;

namespace EFCore.CodeGenerator
{
    public static class Generator
    {
        public static IEnumerable<string> Scaffolding(string @namespace, string contextName, string writeCodePath)
        {
            var generator = new DbContextGenerator(@namespace, contextName, writeCodePath);
            generator.WriteTo();

            return generator.WriteAllTextModels.Select(o => o.Code);
        }
    }
}
