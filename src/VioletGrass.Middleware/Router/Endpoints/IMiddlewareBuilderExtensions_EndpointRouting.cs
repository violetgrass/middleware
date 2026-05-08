using System;

using Violet.Middleware.Router;

namespace Violet.Middleware;

public static partial class IMiddlewareBuilderExtensions
{
    public static IMiddlewareBuilder<TContext> UseRouting<TContext>(this IMiddlewareBuilder<TContext> self) where TContext : Context
        => self.Use(EndpointRouter.CreateRoutingSetupMiddlewareFactory(self));

    public static IMiddlewareBuilder<TContext> UseEndpoints<TContext>(this IMiddlewareBuilder<TContext> self, Action<IEndpointRouteBuilder<TContext>> endpoints) where TContext : Context
        => self.Use(EndpointRouter.CreateEndpointDispatcherMiddlewareFactory(self, endpoints));
}