using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
        public static IMiddlewareBuilder<TContext> Use<TContext>(this IMiddlewareBuilder<TContext> self, MiddlewareDelegate<TContext> middleware) where TContext : Context
            => self.Use(next => async context => { await middleware(context); await next(context); });
        public static IMiddlewareBuilder<TContext> Run<TContext>(this IMiddlewareBuilder<TContext> self, MiddlewareDelegate<TContext> handler) where TContext : Context
            => self.Use(next => handler);

        public static IMiddlewareBuilder<TContext> Use<TMiddleware, TContext>(this IMiddlewareBuilder<TContext> self) where TContext : Context where TMiddleware : IMiddleware<TContext>, new()
            => self.Use(self.ServiceProvider is null ? new TMiddleware() : ActivatorUtilities.CreateInstance<TMiddleware>(self.ServiceProvider));
    }
}