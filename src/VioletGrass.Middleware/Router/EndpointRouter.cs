using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VioletGrass.Middleware.Router
{
    internal partial class EndpointRouter
    {
        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateRoutingSetupMiddlewareFactory<TContext>(IMiddlewareBuilder<TContext> middlewareBuilder) where TContext : Context
        {
            if (middlewareBuilder is null)
            {
                throw new ArgumentNullException(nameof(middlewareBuilder));
            }

            // Setup the endpoint route builder
            var endpointRouteBuilder = EnsureEndpointRouteBuilder(middlewareBuilder);
            endpointRouteBuilder.BuildEndpointRoutes();

            // return the factory
            return RoutingSetupMiddlewareFactory;

            MiddlewareDelegate<TContext> RoutingSetupMiddlewareFactory(MiddlewareDelegate<TContext> next)
            {
                var endpointRoutes = endpointRouteBuilder.EndpointRoutes;

                return RoutingSetupMiddleware;

                async Task RoutingSetupMiddleware(TContext context)
                {
                    AddEndpointFeature(context, endpointRoutes);

                    await next(context);
                }
            }
        }

        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateEndpointDispatcherMiddlewareFactory<TContext>(IMiddlewareBuilder<TContext> middlewareBuilder, Action<IEndpointRouteBuilder<TContext>> configure) where TContext : Context
        {
            if (middlewareBuilder is null)
            {
                throw new ArgumentNullException(nameof(middlewareBuilder));
            }

            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var endpointDispatcherId = Guid.NewGuid();

            var endpointRouteBuilder = EnsureEndpointRouteBuilder(middlewareBuilder);
            // each UseEndpoints only executes the endpoints withing the provided configuration and skip all other endpoints. If queried outside of local configuration, return the element
            endpointRouteBuilder.PushPredicateContext(context => context.Feature<EndpointDispatcherFeature>()?.DispatcherId is null || context.Feature<EndpointDispatcherFeature>()?.DispatcherId == endpointDispatcherId);
            configure(endpointRouteBuilder);
            endpointRouteBuilder.PopPredicateContext();
            endpointRouteBuilder.BuildEndpointRoutes();

            return EndpointDispatcherMiddlewareFactory; // Terminal Middleware

            MiddlewareDelegate<TContext> EndpointDispatcherMiddlewareFactory(MiddlewareDelegate<TContext> next)
            {
                var endpointRoutes = endpointRouteBuilder.EndpointRoutes;

                return EndpointDispatcherMiddleware;

                async Task EndpointDispatcherMiddleware(TContext context)
                {
                    var feature = EnsureEndpointFeature(context, endpointRoutes);

                    // select only endpoints in the current configuration
                    context.Features.Set(new EndpointDispatcherFeature() { DispatcherId = endpointDispatcherId });

                    if (feature.TryGetEndpoint(context, out var endpoint))
                    {
                        context.Features.Set<EndpointDispatcherFeature>(null);

                        await endpoint.MiddlewareDelegate(context);
                    }
                    else
                    {
                        context.Features.Set<EndpointDispatcherFeature>(null);

                        await next(context);
                    }
                }
            }
        }

        private static DefaultEndpointRouteBuilder<TContext> EnsureEndpointRouteBuilder<TContext>(IMiddlewareBuilder<TContext> middlewareBuilder) where TContext : Context
        {
            if (middlewareBuilder is null)
            {
                throw new ArgumentNullException(nameof(middlewareBuilder));
            }

            if (!middlewareBuilder.Properties.TryGetValue(DefaultEndpointRouteBuilder<TContext>.PropertyName, out var endpointRouteBuilder))
            {
                endpointRouteBuilder = new DefaultEndpointRouteBuilder<TContext>(middlewareBuilder.ServiceProvider);

                middlewareBuilder.Properties.Add(DefaultEndpointRouteBuilder<TContext>.PropertyName, endpointRouteBuilder);
            }

            return endpointRouteBuilder as DefaultEndpointRouteBuilder<TContext>;
        }

        private static EndpointFeature<TContext> EnsureEndpointFeature<TContext>(TContext context, IEnumerable<EndpointPredicate<TContext>> endpointRoutes) where TContext : Context
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var endpointFeature = context.Features.Get<EndpointFeature<TContext>>();

            if (endpointFeature == null)
            {
                endpointFeature = AddEndpointFeature(context, endpointRoutes);
            }

            return endpointFeature;
        }

        private static EndpointFeature<TContext> AddEndpointFeature<TContext>(TContext context, IEnumerable<EndpointPredicate<TContext>> endpointRoutes) where TContext : Context
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var feature = new EndpointFeature<TContext>(endpointRoutes);

            var endpointFeature = context.Features.Set(feature);

            return endpointFeature;
        }
    }
}