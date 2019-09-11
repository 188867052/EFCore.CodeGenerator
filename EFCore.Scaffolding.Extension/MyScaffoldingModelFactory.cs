namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Design.Internal;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

    public class MyScaffoldingModelFactory
        : RelationalScaffoldingModelFactoryBase
    {
        public MyScaffoldingModelFactory([NotNull] IOperationReporter reporter, [NotNull] ICandidateNamingService candidateNamingService, [NotNull] IPluralizer pluralizer, [NotNull] ICSharpUtilities cSharpUtilities, [NotNull] IScaffoldingTypeMapper scaffoldingTypeMapper) : base(reporter, candidateNamingService, pluralizer, cSharpUtilities, scaffoldingTypeMapper)
        {
            this.Data = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Data { get; }

        protected override void SetKeyDictionary(KeyBuilder keyBuilder, DatabaseTable table)
        {
        }

        protected override void SetTableDictionary(string entityTypeName, DatabaseTable table)
        {
            this.Data.TryAdd($"{entityTypeName}.PrimaryKey", table.PrimaryKey.Columns.FirstOrDefault().Name);
            this.Data.Add(entityTypeName, table.Name);
        }

        protected override void SetDictionary(PropertyBuilder property, DatabaseColumn column)
        {
            this.Data.Add($"{property.Metadata.DeclaringType.Name}.{property.Metadata.Name}", column.Name);
        }
    }
}
