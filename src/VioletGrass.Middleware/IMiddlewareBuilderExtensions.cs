using System;
using System.Threading.Tasks;

namespace VioletGrass.Middleware
{
    public static partial class IMiddlewareBuilderExtensions
    {
        public static IMiddlewareBuilder Use(this IMiddlewareBuilder self, IMiddleware middleware)
            => self.Use(middleware.InvokeAsync);

        public static IMiddlewareBuilder Use(this IMiddlewareBuilder self, Func<Context, MiddlewareDelegate, Task> middleware)
            => self.Use(factoryInputNext =>
            {
                return context => middleware(context, factoryInputNext);
            });

        public static IMiddlewareBuilder Use<TMiddleware>(this IMiddlewareBuilder self) where TMiddleware : IMiddleware, new()
            => self.Use(new TMiddleware());
    }
}