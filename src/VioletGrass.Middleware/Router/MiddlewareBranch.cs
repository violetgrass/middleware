using System;

namespace VioletGrass.Middleware.Router
{
    internal struct MiddlewareBranch<TContext> where TContext : Context
    {
        public MiddlewareBranch(Predicate<TContext> isApplicable, MiddlewareDelegate<TContext> branch)
        {
            IsApplicable = isApplicable ?? throw new ArgumentNullException(nameof(isApplicable));
            Branch = branch ?? throw new ArgumentNullException(nameof(branch));
        }
        public Predicate<TContext> IsApplicable { get; }
        public MiddlewareDelegate<TContext> Branch { get; }
    }
}