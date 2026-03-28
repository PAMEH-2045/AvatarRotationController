using System;

namespace MEHelper.ReflectionWrapper.Exceptions
{
    public class AdapterNotFoundException : Exception
    {
        internal AdapterNotFoundException(Type adapterType)   
            : base($"[ReflectionWrapper] Adapter of type '{adapterType}' is not in the cache") { }
    }
}