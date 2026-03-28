using MEHelper.ReflectionWrapper.Iterators;

namespace MEHelper.ReflectionWrapper
{
    internal static class WrapperFacade
    {
        public static ExpressionAdapter BuildAdapter<T>(T obj) where T : class
        {
            var (fields, methods) = FieldsAndMethodsIterator.Iterate(obj.GetType());
            var adapter = new ExpressionAdapter(obj, fields, methods);

            return adapter;
        }
    }
}