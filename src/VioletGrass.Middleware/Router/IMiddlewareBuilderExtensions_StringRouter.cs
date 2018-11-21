using System;
using VioletGrass.Middleware.Router;

namespace VioletGrass.Middleware
{
    public static class IMiddlewareBuilderExtensions_StringRouter
    {
        public static IMiddlewareBuilder<TContext> UseRoutingKey<TContext>(this IMiddlewareBuilder<TContext> self, Func<Context, string> routingKeySelector) where TContext : Context
        {
            return self.Use(StringRouter.CreateMiddlewareFactoryForRoutingKeyExtractor<TContext>(routingKeySelector));
        }

        public static IMiddlewareBuilder<TContext> UseRoutingDataExtractor<TContext>(this IMiddlewareBuilder<TContext> self, params string[] routePatterns) where TContext : Context
        {
            return self.Use(StringRouter.CreateMiddlewareFactoryForRouteDataExtractor<TContext>(routePatterns));
        }
    }
}