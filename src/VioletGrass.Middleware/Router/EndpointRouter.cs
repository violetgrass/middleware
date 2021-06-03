using System;
using System.Threading.Tasks;

namespace VioletGrass.Middleware.Router
{
    internal class EndpointRouter
    {
        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateRoutingSetupMiddlewareFactory<TContext>(IMiddlewareBuilder<TContext> middlewareBuilder) where TContext : Context
        {
            if (middlewareBuilder is null)
            {
                throw new ArgumentNullException(nameof(middlewareBuilder));
            }

            // Setup the endpoint route builder
            var endpointRouteBuilder = EnsureEndpointRouteBuilder(middlewareBuilder);

            // return the factory
            return RoutingSetupMiddlewareFactory;

            MiddlewareDelegate<TContext> RoutingSetupMiddlewareFactory(MiddlewareDelegate<TContext> next)
            {
                return RoutingSetupMiddleware;

                async Task RoutingSetupMiddleware(TContext context)
                {
                    AddEndpointFeature(context, endpointRouteBuilder);

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

            var endpointRouteBuilder = EnsureEndpointRouteBuilder(middlewareBuilder);
            configure(endpointRouteBuilder);

            return EndpointDispatcherMiddlewareFactory; // Terminal Middleware

            MiddlewareDelegate<TContext> EndpointDispatcherMiddlewareFactory(MiddlewareDelegate<TContext> next)
            {
                return EndpointDispatcherMiddleware;

                async Task EndpointDispatcherMiddleware(TContext context)
                {
                    var feature = EnsureEndpointFeature(context, endpointRouteBuilder);

                    if (feature.TryGetEndpoint(context, out var endpoint))
                    {
                        await endpoint.MiddlewareDelegate(context);
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

        private static EndpointFeature<TContext> EnsureEndpointFeature<TContext>(TContext context, DefaultEndpointRouteBuilder<TContext> endpointRouteBuilder) where TContext : Context
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var endpointFeature = context.Features.Get<EndpointFeature<TContext>>();

            if (endpointFeature == null)
            {
                endpointFeature = AddEndpointFeature(context, endpointRouteBuilder);
            }

            return endpointFeature;
        }

        private static EndpointFeature<TContext> AddEndpointFeature<TContext>(TContext context, DefaultEndpointRouteBuilder<TContext> endpointRouteBuilder) where TContext : Context
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (endpointRouteBuilder is null)
            {
                throw new ArgumentNullException(nameof(endpointRouteBuilder));
            }

            var endpointFeature = context.Features.Set(endpointRouteBuilder.BuildFeature());

            return endpointFeature;
        }
    }
}