using System;

namespace VioletGrass.Middleware.Router
{
    internal struct MiddlewareBranch
    {
        public MiddlewareBranch(Predicate<Context> isApplicable, MiddlewareDelegate branch)
        {
            IsApplicable = isApplicable ?? throw new ArgumentNullException(nameof(isApplicable));
            Branch = branch ?? throw new ArgumentNullException(nameof(branch));
        }
        public Predicate<Context> IsApplicable { get; }
        public MiddlewareDelegate Branch { get; }
    }
}