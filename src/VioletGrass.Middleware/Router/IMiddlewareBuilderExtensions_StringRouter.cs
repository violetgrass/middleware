using System;

namespace VioletGrass.Middleware.Router
{
    public static class IMiddlewareBuilderExtensions_StringRouter
    {
        public static IMiddlewareBuilder UseRoutingKey(this IMiddlewareBuilder self, Func<Context, string> routingKeySelector)
        {
            return self.Use(StringRouter.CreateMiddlewareFactoryForRoutingKeyExtractor(routingKeySelector));
        }

        public static IMiddlewareBuilder UseRoutingDataExtractor(this IMiddlewareBuilder self, string[] routePatterns)
        {
            return self.Use(StringRouter.CreateMiddlewareFactoryForRouteDataExtractor(routePatterns));
        }
    }
}