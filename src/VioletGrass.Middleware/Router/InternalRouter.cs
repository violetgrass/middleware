using System;
using System.Linq;

namespace VioletGrass.Middleware.Router
{
    internal static class InternalRouter
    {
        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateMiddlewareFactory<TContext>(IMiddlewareBuilder<TContext> self, Route<TContext>[] routes) where TContext : Context
        {
            return next =>
            {
                // as part of the factory for the routing step, built the middleware stacks for each branch
                var branches = BuildMiddlewareBranches(self, routes);

                // integrate a middleware which selects the built branches based on their predicate.
                return CreateMiddleware(next, branches);
            };
        }

        private static MiddlewareDelegate<TContext> CreateMiddleware<TContext>(MiddlewareDelegate<TContext> next, MiddlewareBranch<TContext>[] branches) where TContext : Context
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (branches == null)
            {
                throw new ArgumentNullException(nameof(branches));
            }

            return async context =>
            {
                bool goNext = true;

                foreach (var branch in branches)
                {
                    if (branch.IsApplicable(context))
                    {
                        await branch.Branch(context);

                        goNext = false;
                        break;
                    }
                }

                if (goNext)
                {
                    await next(context); // continue after all routes failed
                }
            };
        }

        private static MiddlewareBranch<TContext>[] BuildMiddlewareBranches<TContext>(IMiddlewareBuilder<TContext> parentBuilder, Route<TContext>[] routes) where TContext : Context
        {
            if (parentBuilder == null)
            {
                throw new ArgumentNullException(nameof(parentBuilder));
            }

            if (routes == null)
            {
                throw new ArgumentNullException(nameof(routes));
            }

            return routes.Select(route =>
            {
                var branchBuilder = parentBuilder.New();

                route.MiddlewareBuilderForRoute(branchBuilder);

                var branchStack = branchBuilder.Build();

                return new MiddlewareBranch<TContext>(route.IsApplicable, branchStack);
            }).ToArray();
        }
    }
}