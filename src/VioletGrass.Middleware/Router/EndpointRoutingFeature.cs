using System.Collections.Generic;
using System.Linq;

namespace VioletGrass.Middleware.Router
{
    public class EndpointRoutingFeature<TContext> where TContext : Context
    {
        public IEnumerable<EndpointPredicateRoute<TContext>> EndpointRoutes { get; }

        public EndpointRoutingFeature(IEnumerable<EndpointPredicateRoute<TContext>> endpointRoutes)
        {
            EndpointRoutes = endpointRoutes ?? throw new System.ArgumentNullException(nameof(endpointRoutes));
        }

        public bool TryGetEndpoint(TContext context, out Endpoint<TContext> endpoint)
        {
            Endpoint<TContext> firstMatchedEndpoint = null;
            try
            {
                var firstMatchedEndpointPredicateRoute = EndpointRoutes.FirstOrDefault(route => route.Predicates.All(p => p(context)));

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