using System;
using System.Threading.Tasks;

namespace VioletGrass.Middleware
{
    public static partial class IEndpointRouteBuilderExtensions
    {
        public static void MapLambda<TContext>(this IEndpointRouteBuilder<TContext> self, string displayName, Action<TContext> lambda) where TContext : Context
            => self.Map()
                .WithDisplayName(displayName)
                .WithMiddlewareDelegate((context) => { lambda(context); return Task.CompletedTask; });

        public static void MapLambda<TContext>(this IEndpointRouteBuilder<TContext> self, string name, MiddlewareDelegate<TContext> lambda) where TContext : Context
            => self.Map()
                .WithDisplayName(name)
                .WithMiddlewareDelegate(lambda);
    }
}