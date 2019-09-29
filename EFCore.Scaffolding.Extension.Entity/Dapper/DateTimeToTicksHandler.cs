namespace EFCore.Scaffolding.Extension.Entity.Dapper
{
    using System;

    public class DateTimeToTicksHandler : ValueHandlerBase<DateTime>
    {
        public override DateTime Parse(object value)
        {
            return value switch
            {
                long ticks => new DateTime(ticks),
                DateTime time => time,
                _ => throw new NotSupportedException("Not Supported Type."),
            };
        }
    }
}