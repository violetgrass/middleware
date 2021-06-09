using System;
using VioletGrass.Middleware.Endpoints;

namespace VioletGrass.Middleware
{
    public static partial class IEndpointRouteBuilderExtensions
    {
        public static IEndpointBuilder<TContext> MapControllerAction<TContext, TController>(this IEndpointRouteBuilder<TContext> self, TController instance, string methodName) where TContext : Context
            => self.MapControllerAction(() => instance, methodName);

        public static IEndpointBuilder<TContext> MapControllerAction<TContext, TController>(this IEndpointRouteBuilder<TContext> self, Func<TController> instanceFactory, string methodName) where TContext : Context
            => ControllerEndpoint.MapAction(self, instanceFactory, methodName);

        public static void MapController<TContext, TController>(this IEndpointRouteBuilder<TContext> self, TController instance) where TContext : Context
            => self.MapController(() => instance);

        public static void MapController<TContext, TController>(this IEndpointRouteBuilder<TContext> self, Func<TController> instanceFactory) where TContext : Context
            => ControllerEndpoint.MapController(self, instanceFactory);
    }
}