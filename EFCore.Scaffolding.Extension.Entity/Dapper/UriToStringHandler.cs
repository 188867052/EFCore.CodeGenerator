namespace EFCore.Scaffolding.Extension.Entity.Dapper
{
    using System;

    public class UriToStringHandler : ValueHandlerBase<Uri>
    {
        public override Uri Parse(object value)
        {
            return value == null ? null : new Uri(value.ToString());
        }
    }
}