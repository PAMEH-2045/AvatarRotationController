using MEHelper.ReflectionWrapper.DTO;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace MEHelper.ReflectionWrapper.Factories
{
    internal class FieldDTOFactory
    {
        public static FieldDTO Create(FieldInfo fieldInfo, ParameterExpression instanceParam)
        {
            // currently unsupported fields: readonly, const
            //Debug.Log($"[ReflectionWrapper] Creating expression accessros for a '{fieldInfo.Name}' in type '{fieldInfo.DeclaringType}'");
            MemberExpression fieldAccess;
            if (fieldInfo.IsStatic)
            {
                fieldAccess = Expression.Field(null, fieldInfo);
            }
            else
            {
                var castInstance = Expression.Convert(instanceParam, fieldInfo.DeclaringType);
                fieldAccess = Expression.Field(castInstance, fieldInfo);
            }  
            
            var getter = GetterFactory.Create(fieldAccess, instanceParam);
            var setter = SetterFactory.Create(fieldAccess, instanceParam, fieldInfo.FieldType);

            return new FieldDTO(getter, setter, fieldInfo.IsStatic);
        }
    }
}
