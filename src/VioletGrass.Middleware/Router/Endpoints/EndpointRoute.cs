using System;

namespace VioletGrass.Middleware.Router
{
    public class EndpointRoute<TContext> where TContext : Context
    {
        public EndpointRoute(Predicate<TContext>[] predicates, IEndpointBuilder<TContext> endpointBuilder)
        {
            Predicates = predicates ?? throw new ArgumentNullException(nameof(predicates));
            EndpointBuilder = endpointBuilder ?? throw new ArgumentNullException(nameof(endpointBuilder));
        }

        public Predicate<TContext>[] Predicates { get; set; }
        public IEndpointBuilder<TContext> EndpointBuilder { get; }
        public Endpoint<TContext> Endpoint { get; set; }
    }
}