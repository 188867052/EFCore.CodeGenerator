namespace Entities
{
    using System.Collections.Generic;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1509:Opening braces should not be preceded by blank line", Justification = "<挂起>")]
    public static class DefaultValueSqlModel
    {
        public static Dictionary<string, string> Mapping = new Dictionary<string, string>
        {
            { "log.identifier", "identifier" },
        };
    }
}
