using System;
using Newtonsoft.Json;
using VioletGrass.Middleware.Features;

namespace VioletGrass.Middleware.Serializer.Json
{
    internal static class JsonArgumentSerializer
    {
        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateMiddlewareFactoryForJsonSerializer<T, TContext>(Func<TContext, string> contentSelector, string argumentName) where TContext : Context
        {
            return next => CreateMiddlewareForJsonSerializer<T, TContext>(next, contentSelector, argumentName);
        }

        private static MiddlewareDelegate<TContext> CreateMiddlewareForJsonSerializer<T, TContext>(MiddlewareDelegate<TContext> next, Func<TContext, string> contentSelector, string argumentName) where TContext : Context
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
