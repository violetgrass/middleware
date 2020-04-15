using System;

namespace VioletGrass.Middleware.Router
{
    internal class EndpointRouter
    {
        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateRoutingSetupMiddlewareFactory<TContext>(IMiddlewareBuilder<TContext> builder) where TContext : Context
        {
            // Setup the IMiddlewareBuilder
            var endpointRouteBuilder = new EndpointRouteBuilder<TContext>();
            builder.Properties.Add(EndpointRouteBuilder<TContext>.PropertyName, endpointRouteBuilder);

            // Setup the factory to create the middleware itself
            return next =>
            {
                return CreateRoutingSetupMiddleware(next);
            };

            MiddlewareDelegate<TContext> CreateRoutingSetupMiddleware(MiddlewareDelegate<TContext> next)
            {
                return async context =>
                {
                    context.Features.Set(endpointRouteBuilder.BuildFeature());

                    await next(context);
                };
            }
        }

        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateEndpointMapperMiddlewareFactory<TContext>(IMiddlewareBuilder<TContext> self, Action<IEndpointRouteBuilder<TContext>> configure) where TContext : Context
        {
            var endpointRouteBuilder = self.Properties[EndpointRouteBuilder<TContext>.PropertyName] as EndpointRouteBuilder<TContext>;
            endpointRouteBuilder.PushMiddlewareBuilder(self);
            configure(endpointRouteBuilder);
            endpointRouteBuilder.PopMiddlewareBuilder();

            return next => // Terminal Middleware
            {
                return async context =>
                {
                    var feature = context.Feature<EndpointFeature<TContext>>();

                    if (feature.TryGetEndpoint(context, out var endpoint))
                    {
                        await endpoint.DispatcherAsync(context);
                    }
                };
            };
        }
    }
}