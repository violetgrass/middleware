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
            EndpointRoutes.Add(new EndpointPredicate<TContext>(_predicateStack.ToArray(), endpointBuilder));
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

                endpointPredicate.Endpoint = endpoint;
            }

            return new EndpointFeature<TContext>(EndpointRoutes);

        }
    }
}