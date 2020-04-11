namespace EFCore.CodeGenerator.Entity.Dapper
{
    using System;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

    public static class ValueConverter
    {
        private static readonly DateTimeToTicksConverter dateTimeToTicks = new DateTimeToTicksConverter();
        private static readonly BoolToStringConverter boolToString = new BoolToStringConverter(bool.FalseString, bool.TrueString);
        private static readonly BoolToZeroOneConverter<int> boolToZeroOne = new BoolToZeroOneConverter<int>();
        private static readonly UriToStringConverter uriToString = new UriToStringConverter();

        public static object GetConvertedValue<T>(T obj, PropertyInfo propertyInfo, Property property)
        {
            var v = propertyInfo.GetValue(obj);
            switch (property.ConverterEnum)
            {
                case ValueConverterEnum.None:
                case ValueConverterEnum.EnumToString:
                    return v;
                case ValueConverterEnum.BoolToString:
                    return boolToString.ConvertToProviderExpression.Compile()((bool)v);
                case ValueConverterEnum.DateTimeToTicks:
                    return dateTimeToTicks.ConvertToProviderExpression.Compile()((DateTime)v);
                case ValueConverterEnum.BoolToZeroOne:
                    return boolToZeroOne.ConvertToProviderExpression.Compile()((bool)v);
                case ValueConverterEnum.UriToString:
                    return uriToString.ConvertToProviderExpression.Compile()((Uri)v);
                default:
                    throw new NotSupportedException($"Not Supported Converter: {property.ConverterEnum}.");
            }
        }
    }
}