namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Design.Internal;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

    public class MyScaffoldingModelFactory
        : RelationalScaffoldingModelFactoryBase
    {
        public MyScaffoldingModelFactory(
            [NotNull] IOperationReporter reporter,
            [NotNull] ICandidateNamingService candidateNamingService,
            [NotNull] IPluralizer pluralizer,
            [NotNull] ICSharpUtilities cSharpUtilities,
            [NotNull] IScaffoldingTypeMapper scaffoldingTypeMapper,
            [NotNull] LoggingDefinitions loggingDefinitions
            )
            : base(reporter,
                  candidateNamingService,
                  pluralizer,
                  cSharpUtilities,
                  scaffoldingTypeMapper, loggingDefinitions)
        {
        }
    }
}
