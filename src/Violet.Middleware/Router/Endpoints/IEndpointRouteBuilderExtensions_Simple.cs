using System;
using System.Threading.Tasks;

using Violet.Middleware.Handler;

namespace Violet.Middleware;

public static partial class IEndpointRouteBuilderExtensions
{
    public static IEndpointBuilder<TContext> MapLambda<TContext>(this IEndpointRouteBuilder<TContext> self, string displayName, Action<TContext> lambda) where TContext : Context
        => self.Map()
            .WithDisplayName(displayName)
            .WithMiddlewareDelegate((context) => { lambda(context); return Task.CompletedTask; });

    public static IEndpointBuilder<TContext> MapLambda<TContext>(this IEndpointRouteBuilder<TContext> self, string name, MiddlewareDelegate<TContext> lambda) where TContext : Context
        => self.Map()
            .WithDisplayName(name)
            .WithMiddlewareDelegate(lambda);

    public static IEndpointBuilder<TContext> Map<TContext>(this IEndpointRouteBuilder<TContext> self, string name, Delegate handler) where TContext : Context
        => self.Map()
            .WithDisplayName(name)
            .WithMiddlewareDelegate(
                MiddlewareDelegateFactory.Create<TContext>(
                    handler,
                    new MiddlewareDelegateOptions<TContext> { ServiceProvider = self.ServiceProvider }));
}