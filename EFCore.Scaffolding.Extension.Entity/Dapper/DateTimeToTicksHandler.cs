namespace EFCore.Scaffolding.Extension.Entity.Dapper
{
    using System;
    using System.Data;
    using global::Dapper;

    public class DateTimeToTicksHandler : SqlMapper.TypeHandler<DateTime>
    {
        public override DateTime Parse(object value)
        {
            switch (value)
            {
                case long ticks:
                    return new DateTime(ticks);
                case DateTime time:
                    return time;
                default:
                    throw new NotSupportedException("Not Supported Type.");
            }
        }

        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            switch (parameter.DbType)
            {
                case DbType.Int64:
                    parameter.Value = value.Ticks;
                    break;
                default:
                    throw new NotSupportedException("Not Supported Type.");
            }
        }
    }
}