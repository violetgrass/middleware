using System;
using VioletGrass.Middleware.Serializer.Json;

namespace VioletGrass.Middleware
{
    public static class IMiddlewareBuilderExtensions
    {
        public static IMiddlewareBuilder UseJsonSerializer<T>(this IMiddlewareBuilder self, Func<Context, string> contentSelector, string argumentName)
        {
            return self.Use(JsonArgumentSerializer.CreateMiddlewareFactoryForJsonSerializer<T>(contentSelector, argumentName));
        }
    }
}
