namespace EFCore.Scaffolding.Extension.Entity.Dapper
{
    using System;

    public class UriToStringHandler : ValueHandlerBase<Uri>
    {
        public override Uri Parse(object value)
        {
            if (value != null && value.ToString().StartsWith(Uri.UriSchemeHttp))
            {
                return new Uri(value.ToString());
            }

            return null;
        }
    }
}