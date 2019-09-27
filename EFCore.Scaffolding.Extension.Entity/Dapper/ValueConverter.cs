namespace EFCore.Scaffolding.Extension.Entity.Dapper
{
    using System;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

    public static class ValueConverter
    {
        private static readonly DateTimeToTicksConverter dateTimeToTicks = new DateTimeToTicksConverter();

        public static object GetConvertedValue<T>(T obj, PropertyInfo propertyInfo, Models.Property property)
        {
            var v = propertyInfo.GetValue(obj);
            switch (property.Converter)
            {
                case null:
                case "EnumToString":
                    return v;
                case "DateTimeToTicks":
                    return dateTimeToTicks.ConvertToProviderExpression.Compile()((DateTime)v);
                default:
                    throw new NotSupportedException("Not Supported Converter.");
            }
        }
    }
}