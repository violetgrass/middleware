using System;
using System.Linq;
using System.Threading.Tasks;

namespace VioletGrass.Middleware.Router
{
    internal static class InternalRouter
    {
        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateMiddlewareFactory<TContext>(IMiddlewareBuilder<TContext> self, Route<TContext>[] routes) where TContext : Context
        {
            // as part of the builder for the routing step, built the middleware stacks for each branch
            var branches = BuildMiddlewareBranches(self, routes);

            return BranchMiddlewareFactory;

            // integrate a middleware which selects the built branches based on their predicate.
            MiddlewareDelegate<TContext> BranchMiddlewareFactory(MiddlewareDelegate<TContext> next)
            {
                return CreateMiddleware(next, branches);
            }
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

            return BranchMiddleware;

            async Task BranchMiddleware(TContext context)
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
                // setup builder and invoke configuration pipeline
                var branchBuilder = parentBuilder.New();

                // register route context with relevant features (TODO: abstraction layering)
                foreach (var property in parentBuilder.Properties.Where(x => x.Value is IRouteContextAware<TContext>))
                {
                    branchBuilder.Properties.Add(property);

                    var routeContextAware = property.Value as IRouteContextAware<TContext>;

                    routeContextAware.PushRouteContext(route);
                }

                route.MiddlewareBuilderForRoute(branchBuilder);

                // build the stack
                var branchStack = branchBuilder.Build();

                // unregister route context with relevant features
                foreach (var property in parentBuilder.Properties.Where(x => x.Value is IRouteContextAware<TContext>))
                {
                    var routeContextAware = property.Value as IRouteContextAware<TContext>;

                    routeContextAware.PopRouteContext();
                }

                return new MiddlewareBranch<TContext>(route.IsApplicable, branchStack);
            }).ToArray();
        }
    }
}