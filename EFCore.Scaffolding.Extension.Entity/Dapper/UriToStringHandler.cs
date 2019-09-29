namespace EFCore.Scaffolding.Extension.Entity.Dapper
{
    using System;

    public class UriToStringHandler : ValueHandlerBase<Uri>
    {
        public override Uri Parse(object value)
        {
            if (value != null)
            {
                return new Uri(value.ToString());
            }

            return null;
        }
    }
}