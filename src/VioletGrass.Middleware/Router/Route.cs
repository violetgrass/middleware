using System;
using VioletGrass.Middleware;

namespace VioletGrass.Middleware.Router
{
    public struct Route
    {
        public Predicate<Context> IsApplicable { get; }
        public Action<IMiddlewareBuilder> MiddlewareBuilderForRoute { get; }
        public Route(Predicate<Context> isApplicable, Action<IMiddlewareBuilder> middlewareBuilderForBranch)
        {
            IsApplicable = isApplicable;
            MiddlewareBuilderForRoute = middlewareBuilderForBranch;
        }
    }
}