using System;
using System.Linq.Expressions;

namespace MEHelper.ReflectionWrapper.Factories
{
    internal static class GetterFactory
    {
        public static Func<object, object> Create(MemberExpression fieldAccess, ParameterExpression instanceParam)
        {
            var cast = Expression.Convert(fieldAccess, typeof(object));

            var lambda = Expression.Lambda<Func<object, object>>(cast, instanceParam);

            return lambda.Compile();
        }

    }
}
