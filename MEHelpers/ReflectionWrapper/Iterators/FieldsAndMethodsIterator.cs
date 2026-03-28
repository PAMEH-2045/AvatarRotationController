using MEHelper.ReflectionWrapper.DTO;
using MEHelper.ReflectionWrapper.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MEHelper.ReflectionWrapper.Iterators
{
    internal static class FieldsAndMethodsIterator
    {
        public static (Dictionary<string, FieldDTO> fields, Dictionary<string, MethodDTO[]> methods) Iterate(Type instanceType)
        {
            var instanceParam = Expression.Parameter(typeof(object));

            var fieldTasks = IterateFields(instanceParam, instanceType);
            var methodTasks = IterateMethods(instanceParam, instanceType);

            return (fieldTasks, methodTasks);
        }

        private static Dictionary<string, FieldDTO> IterateFields(ParameterExpression instanceParam, Type instanceType)
        {
            var fieldInfos = instanceType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(fieldInfo => !fieldInfo.IsLiteral && !fieldInfo.IsInitOnly);

            return fieldInfos.ToDictionary(
                fieldInfo => fieldInfo.Name, 
                fieldInfo => FieldDTOFactory.Create(fieldInfo, instanceParam)
            );
        }

        private static Dictionary<string, MethodDTO[]> IterateMethods(ParameterExpression instanceParam, Type instanceType)
        {
            var methodInfos = instanceType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(methodInfo => !methodInfo.IsGenericMethod);
            var argsParam = Expression.Parameter(typeof(object[]));

            return methodInfos
                .GroupBy(methodInfo => methodInfo.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(methodInfo => MethodDTOFactory.Create(methodInfo, instanceParam, argsParam)).ToArray()
                );
        }
    }
}
