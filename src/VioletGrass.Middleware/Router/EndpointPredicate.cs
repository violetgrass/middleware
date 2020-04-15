using System;

namespace VioletGrass.Middleware.Router
{
    public class EndpointPredicate<TContext> where TContext : Context
    {
        public EndpointPredicate(Predicate<TContext>[] predicates, Endpoint<TContext> endpoint)
        {
            Predicates = predicates ?? throw new ArgumentNullException(nameof(predicates));
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        public Predicate<TContext>[] Predicates { get; }
        public Endpoint<TContext> Endpoint { get; }
    }
}