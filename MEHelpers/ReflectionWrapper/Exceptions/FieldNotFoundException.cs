using System;

namespace MEHelper.ReflectionWrapper.Exceptions
{
    public class FieldNotFoundException : Exception 
    {
        public FieldNotFoundException(string filedName, string typeName) 
            : base($"[ReflectionWrapper] Unable to find field on name '{filedName}' in type '{typeName}'") { }
    }
}
