using System;

namespace VioletGrass.Middleware.Router
{
    internal class EndpointRouter
    {
        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateRoutingSetupMiddlewareFactory<TContext>(IMiddlewareBuilder<TContext> builder) where TContext : Context
        {
            // Setup the IMiddlewareBuilder
            var endpointBuilder = new EndpointBuilder<TContext>();
            builder.Properties.Add(EndpointBuilder<TContext>.PropertyName, endpointBuilder);

            // Setup the factory to create the middleware itself
            return next =>
            {
                return CreateRoutingSetupMiddleware(next);
            };

            MiddlewareDelegate<TContext> CreateRoutingSetupMiddleware(MiddlewareDelegate<TContext> next)
            {
                return async context =>
                {
                    context.Features.Set(endpointBuilder.BuildFeature());

                    await next(context);
                };
            }
        }

        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateEndpointMapperMiddlewareFactory<TContext>(IMiddlewareBuilder<TContext> self, Action<IEndpointBuilder<TContext>> configure) where TContext : Context
        {
            var endpointBuilder = self.Properties[EndpointBuilder<TContext>.PropertyName] as EndpointBuilder<TContext>;
            endpointBuilder.PushMiddlewareBuilder(self);
            configure(endpointBuilder);
            endpointBuilder.PopMiddlewareBuilder();

            return next => // Terminal Middleware
            {
                return async context =>
                {
                    var feature = context.Feature<EndpointRoutingFeature<TContext>>();

                    if (feature.TryEvaluate(context))
                    {
                        await feature.Endpoint.DispatcherAsync(context);
                    }
                };
            };
        }
    }
}