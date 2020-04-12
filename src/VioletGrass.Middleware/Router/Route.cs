using System;
using VioletGrass.Middleware;

namespace VioletGrass.Middleware
{
    public class Route<TContext> where TContext : Context
    {
        public Predicate<TContext> IsApplicable { get; }
        public Action<IMiddlewareBuilder<TContext>> MiddlewareBuilderForRoute { get; }
        public Route(Predicate<TContext> isApplicable, Action<IMiddlewareBuilder<TContext>> middlewareBuilderForBranch)
        {
            IsApplicable = isApplicable ?? throw new ArgumentNullException(nameof(isApplicable));
            MiddlewareBuilderForRoute = middlewareBuilderForBranch ?? throw new ArgumentNullException(nameof(middlewareBuilderForBranch));
        }
    }
}