using System;

namespace MEHelper.ReflectionWrapper.Exceptions
{
    public class MethodNotFoundExceptions : Exception 
    {
        public MethodNotFoundExceptions(string methodName, string typeName) 
            : base($"[ReflectionWrapper] Unable to find method on name '{methodName}' in type '{typeName}'") { }
    }
}
