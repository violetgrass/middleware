using System;

namespace VioletGrass.Middleware
{
    public interface IEndpointRouteBuilder<TContext> where TContext : Context
    {
        void PushRouteContext(Route<TContext> isApplicable);
        void PopRouteContext();

        void PushPredicateContext(Predicate<TContext> predicate);
        void PopPredicateContext();

        void Map(IEndpointBuilder<TContext> endpointBuilder);
    }
}