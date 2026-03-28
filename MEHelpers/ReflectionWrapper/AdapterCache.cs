using MEHelper.ReflectionWrapper.Exceptions;
using System;
using System.Collections.Generic;

namespace MEHelper.ReflectionWrapper
{
    public static class AdapterCache
    {
        private static readonly Dictionary<Type, ExpressionAdapter> _cache = new Dictionary<Type, ExpressionAdapter>();

        public static void UpdateOrRegister<T>(T obj) where T : class
        {
            var type = obj.GetType();

            if (_cache.ContainsKey(type))
                _cache[type].Obj = obj;
            else
                _cache.Add(
                    type,
                    WrapperFacade.BuildAdapter(obj)
                );
        }

        public static ExpressionAdapter Get<T>() where T : class
        {
            if (!_cache.TryGetValue(typeof(T), out var adapter))
                throw new AdapterNotFoundException(typeof(T));

            return adapter;
        }
    }
}
