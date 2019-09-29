namespace EFCore.Scaffolding.Extension.Entity.Dapper
{
    using System;
    using System.Data;
    using global::Dapper;

    public abstract class ValueHandlerBase<T> : SqlMapper.TypeHandler<T>
    {
        public override abstract T Parse(object value);

        public override void SetValue(IDbDataParameter parameter, T value)
        {
            throw new NotImplementedException();
        }
    }
}