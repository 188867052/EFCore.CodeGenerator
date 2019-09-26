using System;
using System.Collections.Generic;
using System.Reflection;
using Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EFCore.Scaffolding.Extension.Entity.Dapper
{
    public static class ValueConverter
    {
        private static readonly DateTimeToTicksConverter dateTimeToTicks = new DateTimeToTicksConverter();

        public static object GetConvertedValue<T>(T obj, PropertyInfo property)
        {
            var v = property.GetValue(obj);
            var convert = GetConvert<T>(property.Name);
            switch (convert)
            {
                case ConverterEnum.None:
                    return v;
                case ConverterEnum.DateTimeToTicks:
                    return dateTimeToTicks.ConvertToProviderExpression.Compile()((DateTime)v);
                default:
                    throw new NotSupportedException("Not Supported Converter.");
            }
        }

        private static ConverterEnum GetConvert<T>(string property)
        {
            string key = typeof(T).Name + "." + property;
            if (ValueConverterModel.Mapping.ContainsKey(key))
            {
                return ValueConverterModel.Mapping[key];
            }

            return ConverterEnum.None;
        }
    }
}