namespace TestNamespace
{
    using System.Collections.Generic;

     [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1509:Opening braces should not be preceded by blank line", Justification = "<挂起>")]
    public static class MetaData
    {
        public static Dictionary<string, string> Mapping = new Dictionary<string, string>
        {
            { "Person.PrimaryKey", "id" },
            { "Person", "person" },
            { "Person.Id", "id" },
            { "Person.Name", "name" },
            { "Person.Sex", "sex" },
        };
    }
}
