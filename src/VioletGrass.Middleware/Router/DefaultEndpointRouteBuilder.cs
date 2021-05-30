using System;
using System.Collections.Generic;
using System.Linq;

namespace VioletGrass.Middleware.Router
{
    public class DefaultEndpointRouteBuilder<TContext> : IEndpointRouteBuilder<TContext>, IRouteContextAware<TContext> where TContext : Context
    {
        private Stack<Predicate<TContext>> _predicateStack = new Stack<Predicate<TContext>>();
        public static string PropertyName = "EndpointRouteBuilder";

        public List<EndpointPredicate<TContext>> EndpointRoutes { get; } = new List<EndpointPredicate<TContext>>();

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

            EndpointRoutes.Add(new EndpointPredicate<TContext>(Array.Empty<Predicate<TContext>>(), endpointBuilder));
        }

        public void PopRouteContext()
            => PopPredicateContext();

        public void PopPredicateContext()
        {
            _predicateStack.Pop();
        }

        public EndpointFeature<TContext> BuildFeature()
        {
            foreach (var endpointPredicate in EndpointRoutes)
            {
                var endpoint = endpointPredicate.EndpointBuilder.Build();

                endpointPredicate.Predicates = endpointPredicate.EndpointBuilder.Predicates.ToArray();

                endpointPredicate.Endpoint = endpoint;
            }

            return new EndpointFeature<TContext>(EndpointRoutes);
        }
    }
}