using System;
using VioletGrass.Middleware.Router;

namespace VioletGrass.Middleware
{
    public static class IMiddlewareBuilderExtensions_StringRouter
    {
        public static IMiddlewareBuilder<TContext> UseRoutingKey<TContext>(this IMiddlewareBuilder<TContext> self, Func<TContext, string> routingKeySelector, params string[] routePatterns) where TContext : Context
        {
            return self.Use(StringRouter.CreateRoutingKeyMiddlewareFactory<TContext>(routingKeySelector, routePatterns));
        }
    }
}