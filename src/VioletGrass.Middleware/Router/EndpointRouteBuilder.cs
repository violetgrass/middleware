using System;
using System.Collections.Generic;
using System.Linq;

namespace VioletGrass.Middleware.Router
{
    public class EndpointRouteBuilder<TContext> : IEndpointRouteBuilder<TContext> where TContext : Context
    {
        private Stack<Predicate<TContext>> _predicateStack = new Stack<Predicate<TContext>>();
        private Stack<IMiddlewareBuilder<TContext>> _middlewareBuilderStack = new Stack<IMiddlewareBuilder<TContext>>();
        public static string PropertyName = "EndpointRouteBuilder";

        public List<EndpointPredicate<TContext>> EndpointRoutes { get; } = new List<EndpointPredicate<TContext>>();
        public IMiddlewareBuilder<TContext> EndpointMiddlewareBuilder { get => _middlewareBuilderStack.Peek(); }

        public void PushMiddlewareBuilder(IMiddlewareBuilder<TContext> middlewareBuilder)
            => _middlewareBuilderStack.Push(middlewareBuilder);
        public void PopMiddlewareBuilder()
            => _middlewareBuilderStack.Pop();

        public void PushRouteContext(Route<TContext> route)
            => PushPredicateContext(route.IsApplicable);

        public void PushPredicateContext(Predicate<TContext> predicate)
        {
            _predicateStack.Push(predicate);
        }

        public void Map(Endpoint<TContext> endpoint)
        {
            EndpointRoutes.Add(new EndpointPredicate<TContext>(_predicateStack.ToArray(), endpoint));
        }
        public void Map(Endpoint<TContext> endpoint, Predicate<TContext> predicate)
        {
            PushPredicateContext(predicate);
            Map(endpoint);
            PopPredicateContext();
        }

        public void PopRouteContext()
            => PopPredicateContext();

        public void PopPredicateContext()
        {
            _predicateStack.Pop();
        }

        public EndpointFeature<TContext> BuildFeature()
        {
            return new EndpointFeature<TContext>(EndpointRoutes);

        }
    }
}