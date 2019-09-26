namespace EFCore.Scaffolding.Extension.Entity.Dapper
{
    public enum ConverterEnum
    {
        /// <summary>
        /// Not convert.
        /// </summary>
        None,

        /// <summary>
        /// DateTime to ticks converter.
        /// </summary>
        DateTimeToTicks,

        /// <summary>
        /// Enum to string converter.
        /// </summary>
        EnumToString,
    }
}