using System;
using System.Threading.Tasks;

namespace VioletGrass.Middleware
{
    public static partial class IMiddlewareBuilderExtensions
    {
        public static IMiddlewareBuilder<TContext> Use<TContext>(this IMiddlewareBuilder<TContext> self, IMiddleware<TContext> middleware) where TContext : Context
            => self.Use(middleware.InvokeAsync);

        public static IMiddlewareBuilder<TContext> Use<TContext>(this IMiddlewareBuilder<TContext> self, Func<TContext, MiddlewareDelegate<TContext>, Task> middleware) where TContext : Context
            => self.Use(factoryInputNext =>
            {
                return context => middleware(context, factoryInputNext);
            });

        public static IMiddlewareBuilder<TContext> Use<TMiddleware, TContext>(this IMiddlewareBuilder<TContext> self) where TContext : Context where TMiddleware : IMiddleware<TContext>, new()
            => self.Use(new TMiddleware());
    }
}