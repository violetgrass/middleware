using System;
using System.Linq;
using VioletGrass.Middleware.Router;

namespace VioletGrass.Middleware
{
    public static class IMiddlewareBuilderExtensions_Router
    {
        public static IMiddlewareBuilder UseRoutes(this IMiddlewareBuilder self, params Route[] routes)
        {
            if (self == null)
            {
                throw new System.ArgumentNullException(nameof(self));
            }

            if (routes == null)
            {
                throw new System.ArgumentNullException(nameof(routes));
            }

            foreach (var route in routes)
            {
                if (route.IsApplicable == null)
                {
                    throw new System.ArgumentNullException(nameof(route.IsApplicable));
                }

                if (route.MiddlewareBuilderForRoute == null)
                {
                    throw new System.ArgumentNullException(nameof(route.MiddlewareBuilderForRoute));
                }
            }

            return self.Use(next =>
            {
                var branches = routes.Select(r =>
                {
                    var branchBuilder = self.New();

                    r.MiddlewareBuilderForRoute(branchBuilder);

                    var branchStack = branchBuilder.Build();

                    return (isApplicable: r.IsApplicable, branchStack);
                }).ToArray();

                return async context =>
                {
                    bool goNext = true;
                    foreach (var branch in branches)
                    {
                        if (branch.isApplicable(context))
                        {
                            goNext = false;
                            await branch.branchStack(context);
                            break;
                        }
                    }

                    if (goNext)
                    {
                        await next(context); // continue after all routes failed
                    }
                };
            });
        }
    }
}