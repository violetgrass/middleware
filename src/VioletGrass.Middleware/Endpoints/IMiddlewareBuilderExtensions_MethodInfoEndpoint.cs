using System;
using System.Reflection;
using VioletGrass.Middleware.Endpoints;

namespace VioletGrass.Middleware
{
    public static partial class IMiddlewareBuilderExtensions
    {
        // public static IMiddlewareBuilder UseClassEndpoint(this IMiddlewareBuilder self, Type type)
        // {
        //     // route all public methods when a default router is available
        // }

        public static IMiddlewareBuilder UseMethodEndpoint(this IMiddlewareBuilder self, object endpointClass, string methodName)
        {
            if (endpointClass == null)
            {
                throw new ArgumentNullException(nameof(endpointClass));
            }

            if (methodName == null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            return self.Use(MethodInfoEndpoint.CreateMiddlewareFactoryForMethodInfo(endpointClass, methodName));
        }
    }
}