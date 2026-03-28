using System;
using System.Linq.Expressions;

namespace MEHelper.ReflectionWrapper.Factories
{
    internal class SetterFactory
    {
        public static Action<object, object> Create(MemberExpression fieldAccess, ParameterExpression instanceParam, Type fieldType)
        {
            var value = Expression.Parameter(typeof(object));
            var castValue = Expression.Convert(value, fieldType);

            var assign = Expression.Assign(fieldAccess, castValue);

            var lambda = Expression.Lambda<Action<object, object>>(assign, instanceParam, value);

            return lambda.Compile();
        }
    }
}
