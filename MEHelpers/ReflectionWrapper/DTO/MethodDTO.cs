using System;

namespace MEHelper.ReflectionWrapper.DTO
{
    public class MethodDTO
    {
        public readonly Func<object, object[], object> Method;
        public readonly bool IsStatic;
        public readonly Type[] ParamTypes;

        public MethodDTO(Func<object, object[], object> method, bool isStatic, Type[] paramTypes)
        {
            Method = method;
            IsStatic = isStatic;
            ParamTypes = paramTypes;
        }
    }
}
