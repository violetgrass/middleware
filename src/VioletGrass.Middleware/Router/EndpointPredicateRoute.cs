using System;

namespace VioletGrass.Middleware.Router
{
    public class EndpointPredicateRoute<TContext> where TContext : Context
    {
        public EndpointPredicateRoute(Predicate<TContext>[] predicates, Endpoint<TContext> endpoint)
        {
            Predicates = predicates ?? throw new ArgumentNullException(nameof(predicates));
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        public Predicate<TContext>[] Predicates { get; }
        public Endpoint<TContext> Endpoint { get; }
    }
}