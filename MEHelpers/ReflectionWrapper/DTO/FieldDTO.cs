using System;

namespace MEHelper.ReflectionWrapper.DTO
{
    public class FieldDTO
    {
        public readonly Func<object, object> Getter;
        public readonly Action<object, object> Setter;
        public readonly bool IsStatic;

        public FieldDTO(Func<object, object> getter, Action<object, object> setter, bool isStatic)
        {
            Getter = getter;
            Setter = setter;
            IsStatic = isStatic;
        }
    }
}
