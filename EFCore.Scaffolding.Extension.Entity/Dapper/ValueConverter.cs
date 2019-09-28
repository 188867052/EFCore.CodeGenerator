namespace EFCore.Scaffolding.Extension.Entity.Dapper
{
    using System;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

    public static class ValueConverter
    {
        private static readonly DateTimeToTicksConverter dateTimeToTicks = new DateTimeToTicksConverter();
        private static readonly BoolToStringConverter boolToString = new BoolToStringConverter(bool.FalseString, bool.TrueString);
        private static readonly BoolToZeroOneConverter<int> boolToZeroOne = new BoolToZeroOneConverter<int>();

        public static object GetConvertedValue<T>(T obj, PropertyInfo propertyInfo, Models.Property property)
        {
            var v = propertyInfo.GetValue(obj);
            switch (property.ConverterEnum)
            {
                case ConverterEnum.None:
                case ConverterEnum.EnumToString:
                    return v;
                case ConverterEnum.BoolToString:
                    return boolToString.ConvertToProviderExpression.Compile()((bool)v);
                case ConverterEnum.DateTimeToTicks:
                    return dateTimeToTicks.ConvertToProviderExpression.Compile()((DateTime)v);
                case ConverterEnum.BoolToZeroOne:
                    return boolToZeroOne.ConvertToProviderExpression.Compile()((bool)v);
                default:
                    throw new NotSupportedException("Not Supported Converter.");
            }
        }
    }
}