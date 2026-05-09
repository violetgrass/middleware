using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Violet.Middleware.Router;

public class EndpointFeature<TContext> where TContext : Context
{
    public IEnumerable<EndpointRoute<TContext>> Endpoints { get; }

    public EndpointFeature(IEnumerable<EndpointRoute<TContext>> endpoints)
    {
        Endpoints = endpoints ?? throw new System.ArgumentNullException(nameof(endpoints));
    }

    public bool TryGetEndpoint(TContext context, [NotNullWhen(returnValue: true)] out Endpoint<TContext>? endpoint)
    {
        Endpoint<TContext>? firstMatchedEndpoint = null;
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