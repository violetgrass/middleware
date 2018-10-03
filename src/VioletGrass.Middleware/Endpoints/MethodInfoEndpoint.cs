using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VioletGrass.Middleware.Features;

namespace VioletGrass.Middleware.Endpoints
{
    internal static class MethodInfoEndpoint
    {
        public static Func<MiddlewareDelegate, MiddlewareDelegate> CreateMiddlewareFactoryForMethodInfo(object instance, string methodName)
        {
            return next => CreateMiddleware(instance, methodName); // ignore next ... this is a final middleware
        }

        private static MiddlewareDelegate CreateMiddleware(object instance, string methodName)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (methodName == null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            return async context =>
            {
                var methodInfo = instance.GetType().GetMethod(methodName);

                if (methodInfo != null)
                {
                    var arguments = context.Features.Get<Arguments>();

                    if (TryBuildParameters(methodInfo, arguments, out var methodInput))
                    {
                        var result = methodInfo.Invoke(instance, methodInput) as Task;

                        await result;
                    }
                    else
                    {
                        throw new InvalidOperationException("Could fill all method parameters");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Cannot find method {methodName}");
                }
            };
        }

        private static object InstantiateClass(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Activator.CreateInstance(type);
        }

        private static bool TryBuildParameters(MethodInfo methodInfo, Arguments argumentsFromContext, out object[] inputForMethod)
        {
            var result = new List<object>();
            bool success = true;

            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                object argumentValue = null;

                if (
                    argumentsFromContext != null &&
                    argumentsFromContext.TryGetValue(parameterInfo.Name, out var fromContext) &&
                    fromContext.GetType() == parameterInfo.ParameterType)
                {
                    argumentValue = fromContext;
                }
                else
                {
                    success = false;
                }

                result.Add(argumentValue);
            }

            inputForMethod = result.ToArray();

            return success;
        }
    }
}