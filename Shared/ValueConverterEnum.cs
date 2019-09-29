namespace EFCore.Scaffolding.Extension
{
    public enum ValueConverterEnum
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

        /// <summary>
        /// Bool to string converter.
        /// </summary>
        BoolToString,

        /// <summary>
        /// Bool to Zero & One converter.
        /// </summary>
        BoolToZeroOne,

        /// <summary>
        /// Uri to string converter.
        /// </summary>
        UriToString,
    }
}