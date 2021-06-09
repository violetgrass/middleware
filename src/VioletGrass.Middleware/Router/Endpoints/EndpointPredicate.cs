using System;

namespace VioletGrass.Middleware.Router
{
    public class EndpointPredicate<TContext> where TContext : Context
    {
        public EndpointPredicate(Predicate<TContext>[] predicates, IEndpointBuilder<TContext> endpointBuilder)
        {
            Predicates = predicates ?? throw new ArgumentNullException(nameof(predicates));
            EndpointBuilder = endpointBuilder ?? throw new ArgumentNullException(nameof(endpointBuilder));
        }

        public Predicate<TContext>[] Predicates { get; set; }
        public IEndpointBuilder<TContext> EndpointBuilder { get; }
        public Endpoint<TContext> Endpoint { get; set; }
    }
}