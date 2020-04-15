using System.Collections.Generic;
using System.Linq;

namespace VioletGrass.Middleware.Router
{
    public class EndpointFeature<TContext> where TContext : Context
    {
        public IEnumerable<EndpointPredicate<TContext>> Endpoints { get; }

        public EndpointFeature(IEnumerable<EndpointPredicate<TContext>> endpoints)
        {
            Endpoints = endpoints ?? throw new System.ArgumentNullException(nameof(endpoints));
        }

        public bool TryGetEndpoint(TContext context, out Endpoint<TContext> endpoint)
        {
            Endpoint<TContext> firstMatchedEndpoint = null;
            try
            {
                var firstMatchedEndpointPredicateRoute = Endpoints.FirstOrDefault(route => route.Predicates.All(p => p(context)));

                firstMatchedEndpoint = firstMatchedEndpointPredicateRoute?.Endpoint;
            }
            catch
            {
                firstMatchedEndpoint = null;
            }

            endpoint = firstMatchedEndpoint;

            return !(firstMatchedEndpoint is null);
        }
    }
}