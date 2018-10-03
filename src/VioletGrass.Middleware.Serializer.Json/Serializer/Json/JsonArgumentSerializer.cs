using System;
using Newtonsoft.Json;
using VioletGrass.Middleware.Features;

namespace VioletGrass.Middleware.Serializer.Json
{
    internal static class JsonArgumentSerializer
    {
        public static Func<MiddlewareDelegate, MiddlewareDelegate> CreateMiddlewareFactoryForJsonSerializer<T>(Func<Context, string> contentSelector, string argumentName)
        {
            return next => CreateMiddlewareForJsonSerializer<T>(next, contentSelector, argumentName);
        }

        private static MiddlewareDelegate CreateMiddlewareForJsonSerializer<T>(MiddlewareDelegate next, Func<Context, string> contentSelector, string argumentName)
        {
            if (contentSelector == null)
            {
                throw new ArgumentNullException(nameof(contentSelector));
            }

            if (string.IsNullOrWhiteSpace(argumentName))
            {
                throw new ArgumentException("message", nameof(argumentName));
            }

            return async context =>
            {
                var arguments = context.Features.Get<Arguments>() ?? context.Features.Set(new Arguments());

                string content = contentSelector(context);

                var deserializedContent = JsonConvert.DeserializeObject<T>(content);

                arguments.With(argumentName, deserializedContent);

                await next(context);
            };
        }
    }
}
