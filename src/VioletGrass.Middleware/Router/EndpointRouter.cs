using System;

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

            // Setup the IMiddlewareBuilder
            var endpointRouteBuilder = EnsureEndpointRouteBuilder(middlewareBuilder);

            // Setup the factory to create the middleware itself
            return next =>
            {
                return CreateRoutingSetupMiddleware(next);
            };

            MiddlewareDelegate<TContext> CreateRoutingSetupMiddleware(MiddlewareDelegate<TContext> next)
            {
                return async context =>
                {
                    AddEndpointFeature(context, endpointRouteBuilder);

                    await next(context);
                };
            }
        }

        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateEndpointMapperMiddlewareFactory<TContext>(IMiddlewareBuilder<TContext> middlewareBuilder, Action<IEndpointRouteBuilder<TContext>> configure) where TContext : Context
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

            return next => // Terminal Middleware
            {
                return async context =>
                {
                    var feature = EnsureEndpointFeature(context, endpointRouteBuilder);

                    if (feature.TryGetEndpoint(context, out var endpoint))
                    {
                        await endpoint.MiddlewareDelegate(context);
                    }
                };
            };
        }

        public static DefaultEndpointRouteBuilder<TContext> EnsureEndpointRouteBuilder<TContext>(IMiddlewareBuilder<TContext> middlewareBuilder) where TContext : Context
        {
            if (middlewareBuilder is null)
            {
                throw new ArgumentNullException(nameof(middlewareBuilder));
            }

            if (!middlewareBuilder.Properties.TryGetValue(DefaultEndpointRouteBuilder<TContext>.PropertyName, out var endpointRouteBuilder))
            {
                endpointRouteBuilder = new DefaultEndpointRouteBuilder<TContext>();

                middlewareBuilder.Properties.Add(DefaultEndpointRouteBuilder<TContext>.PropertyName, endpointRouteBuilder);
            }

            return endpointRouteBuilder as DefaultEndpointRouteBuilder<TContext>;
        }

        public static EndpointFeature<TContext> EnsureEndpointFeature<TContext>(TContext context, DefaultEndpointRouteBuilder<TContext> endpointRouteBuilder) where TContext : Context
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