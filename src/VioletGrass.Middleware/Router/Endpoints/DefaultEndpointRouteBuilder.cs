using System;
using System.Collections.Generic;
using System.Linq;

namespace VioletGrass.Middleware.Router
{
    public class DefaultEndpointRouteBuilder<TContext> : IEndpointRouteBuilder<TContext>, IRouteContextAware<TContext> where TContext : Context
    {
        public static string PropertyName = "EndpointRouteBuilder";
        private Stack<Predicate<TContext>> _predicateStack = new Stack<Predicate<TContext>>();

        private List<EndpointRoute<TContext>> _endpointRoutes { get; } = new List<EndpointRoute<TContext>>();

        public IEnumerable<EndpointRoute<TContext>> EndpointRoutes { get; private set; } = null;

        public IServiceProvider ServiceProvider { get; }

        public DefaultEndpointRouteBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void PushRouteContext(Route<TContext> route)
            => PushPredicateContext(route.IsApplicable);

        public void PushPredicateContext(Predicate<TContext> predicate)
        {
            _predicateStack.Push(predicate);
        }

        public void Map(IEndpointBuilder<TContext> endpointBuilder)
        {
            // persist a copy of the current predicate stack in endpoint builder
            endpointBuilder.Requires(_predicateStack.ToArray());

            _endpointRoutes.Add(new EndpointRoute<TContext>(Array.Empty<Predicate<TContext>>(), endpointBuilder));
        }

        public void PopRouteContext()
            => PopPredicateContext();

        public void PopPredicateContext()
        {
            _predicateStack.Pop();
        }

        public IEnumerable<EndpointRoute<TContext>> UpdateEndpointRoutes()
        {
            foreach (var endpointPredicate in _endpointRoutes.Where(er => er.Endpoint is null))
            {
                var endpoint = endpointPredicate.EndpointBuilder.Build();

                endpointPredicate.Predicates = endpointPredicate.EndpointBuilder.Predicates.ToArray();

                endpointPredicate.Endpoint = endpoint;
            }

            EndpointRoutes = _endpointRoutes.ToArray(); // copy

            return _endpointRoutes;
        }
    }
}