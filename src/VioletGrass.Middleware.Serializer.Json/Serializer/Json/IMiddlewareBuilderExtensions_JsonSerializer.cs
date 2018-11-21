using System;
using VioletGrass.Middleware.Serializer.Json;

namespace VioletGrass.Middleware
{
    public static class IMiddlewareBuilderExtensions
    {
        public static IMiddlewareBuilder<TContext> UseJsonSerializer<T, TContext>(this IMiddlewareBuilder<TContext> self, Func<TContext, string> contentSelector, string argumentName) where TContext : Context
        {
            return self.Use(JsonArgumentSerializer.CreateMiddlewareFactoryForJsonSerializer<T, TContext>(contentSelector, argumentName));
        }
    }
}
