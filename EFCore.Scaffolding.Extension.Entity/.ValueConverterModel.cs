namespace Entities
{
    using System.Collections.Generic;
    using EFCore.Scaffolding.Extension;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1509:Opening braces should not be preceded by blank line", Justification = "<挂起>")]
    public static class ValueConverterModel
    {
        public static Dictionary<string, ConverterEnum> Mapping = new Dictionary<string, ConverterEnum>
        {
            { "Log.UpdateTimeTicks", ConverterEnum.DateTimeToTicks },
        };
    }
}
