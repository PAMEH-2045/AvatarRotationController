using MEHelper.ReflectionWrapper.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MEHelper.ReflectionWrapper
{
    public class ExpressionAdapter
    {
        public object Obj;
        private readonly Dictionary<string, FieldDTO> _fields;
        private readonly Dictionary<string, MethodDTO[]> _methods;

        public ExpressionAdapter(
            object obj,
            Dictionary<string, FieldDTO> fields,
            Dictionary<string, MethodDTO[]> methods
        )
        {
            Obj = obj;

            _fields = fields;
            _methods = methods;
        }

        public object GetValue(string fieldName)
        {
            if (!_fields.TryGetValue(fieldName, out var dto))
                throw new MissingFieldException($"Getter for field '{fieldName}' not found in adapter of type '{Obj.GetType()}'");

            return dto.Getter(Obj);
        }

        public void SetValue(string fieldName, object value)
        {
            if (!_fields.TryGetValue(fieldName, out var dto))
                throw new MissingFieldException($"Setter for field '{fieldName}' not found in adapter of type '{Obj.GetType()}'");
            
            dto.Setter(Obj, value);
        }

        public object InvokeMethod(string methodName, params object[] args)
        {
            if (!_methods.TryGetValue(methodName, out var overloads))
                throw new MissingMethodException($"Method '{methodName}' not found in adapter of type '{Obj.GetType()}'");

            foreach (var dto in overloads)
            {
                if (dto.ParamTypes.Length != args.Length) continue;

                bool match = true;
                for (int i = 0; i < args.Length; i++)
                {
                    var paramType = dto.ParamTypes[i];
                    var arg = args[i];

                    if (arg == null)
                    {
                        if (paramType.IsValueType && Nullable.GetUnderlyingType(paramType) == null)
                        {
                            match = false;
                            break;
                        }
                    }
                    else if (!paramType.IsAssignableFrom(arg.GetType()))
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    object instance = dto.IsStatic ? null : Obj;

                    return dto.Method(instance, args);
                }

            }

            throw new MissingMethodException($"No overload of '{methodName}' matches arguments ({string.Join(", ", args.Select(a => a?.GetType().Name ?? "null"))})");
        }
    }
}