using System;
using VioletGrass.Middleware.Router;

namespace VioletGrass.Middleware
{
    public static class IMiddlewareBuilderExtensions_StringRouter
    {
        public static IMiddlewareBuilder UseRoutingKey(this IMiddlewareBuilder self, Func<Context, string> routingKeySelector)
        {
            return self.Use(StringRouter.CreateMiddlewareFactoryForRoutingKeyExtractor(routingKeySelector));
        }

        public static IMiddlewareBuilder UseRoutingDataExtractor(this IMiddlewareBuilder self, params string[] routePatterns)
        {
            return self.Use(StringRouter.CreateMiddlewareFactoryForRouteDataExtractor(routePatterns));
        }
    }
}