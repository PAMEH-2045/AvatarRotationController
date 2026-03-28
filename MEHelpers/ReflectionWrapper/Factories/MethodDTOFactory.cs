using MEHelper.ReflectionWrapper.DTO;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace MEHelper.ReflectionWrapper.Factories
{
    internal class MethodDTOFactory
    {
        public static MethodDTO Create(MethodInfo methodInfo, ParameterExpression instanceParam, ParameterExpression argsParam)
        {
            // currently unsupported methods: open/closed generics, in/out/ref params
            //Debug.Log($"[ReflectionWrapper] Creating expression of a method '{methodInfo.Name}' in type '{methodInfo.DeclaringType}'");
            try
            {
                var methodParams = methodInfo.GetParameters();
                var paramTypes = new Type[methodParams.Length];
                var paramExpressions = new Expression[methodParams.Length];

                for (int i = 0; i < methodParams.Length; i++)
                {
                    paramTypes[i] = methodParams[i].ParameterType;
                    var index = Expression.Constant(i);
                    var access = Expression.ArrayIndex(argsParam, index);
                    paramExpressions[i] = Expression.Convert(access, paramTypes[i]);
                }

                Expression callExpression;
                if (methodInfo.IsStatic)
                {
                    callExpression = Expression.Call(methodInfo, paramExpressions);
                }
                else
                {
                    var castInstance = Expression.Convert(instanceParam, methodInfo.DeclaringType);
                    callExpression = Expression.Call(castInstance, methodInfo, paramExpressions);
                }

                Expression body = methodInfo.ReturnType == typeof(void)
                    ? (Expression)Expression.Block(callExpression, Expression.Constant(null))
                    : Expression.Convert(callExpression, typeof(object));

                var lambda = Expression.Lambda<Func<object, object[], object>>(body, instanceParam, argsParam);
                return new MethodDTO(lambda.Compile(), methodInfo.IsStatic, paramTypes);
            }
            catch (Exception ex)
            {
                //Debug.LogWarning($"Unsupported method with name '{methodInfo.Name}' and type '{methodInfo.DeclaringType}'. {ex}");
                return new MethodDTO(
                    (ins, args) => throw new NotImplementedException($"[ReflectionWrapper] Broken method. Name '{methodInfo.Name}', type '{methodInfo.DeclaringType}'"),
                    methodInfo.IsStatic,
                    methodInfo.GetParameters().Select(p => p.ParameterType).ToArray()
                );
            }
        }
    }
}
