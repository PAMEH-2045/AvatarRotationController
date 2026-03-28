using System;

namespace MEHelper.ReflectionWrapper.Exceptions
{
    public class AdapterException : Exception
    {
        internal AdapterException(string message, Exception e)
            : base($"[ReflectionWrapper] {message}", e) { }
    }
}
