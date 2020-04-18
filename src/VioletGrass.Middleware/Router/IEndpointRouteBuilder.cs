using System;

namespace VioletGrass.Middleware
{
    public interface IEndpointRouteBuilder<TContext> where TContext : Context
    {
        void PushPredicateContext(Predicate<TContext> predicate);
        void PopPredicateContext();

        void Map(IEndpointBuilder<TContext> endpointBuilder);
    }
}