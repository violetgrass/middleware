using System;

namespace VioletGrass.Middleware
{
    public interface IEndpointRouteBuilder<TContext> where TContext : Context
    {
        void PushRouteContext(Route<TContext> isApplicable);
        void PopRouteContext();

        void PushPredicateContext(Predicate<TContext> predicate);
        void PopPredicateContext();

        void PushMiddlewareBuilder(IMiddlewareBuilder<TContext> middlewareBuilder);
        void PopMiddlewareBuilder();
        IMiddlewareBuilder<TContext> EndpointMiddlewareBuilder { get; }

        void Map(Endpoint<TContext> endpoint);
        void Map(Endpoint<TContext> endpoint, Predicate<TContext> predicate);
    }
}