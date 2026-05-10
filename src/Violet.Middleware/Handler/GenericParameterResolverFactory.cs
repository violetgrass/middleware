using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Violet.Middleware.Handler;

public class GenericParameterResolverFactory<TContext> : IParameterResolverFactory<TContext> where TContext : Context
{
    public bool TryGetResolver(ParameterInfo parameter, MiddlewareDelegateOptions<TContext> options, [NotNullWhen(returnValue: true)] out IParameterResolver<TContext>? resolver)
    {
        resolver = null;

        // by explicit [From<Feature>] attribute
        if (parameter.GetCustomAttribute(typeof(FromAttribute<>)) is Attribute attribute)
        {
            var featureTypeName = attribute.GetType().GetGenericArguments()[0].Name;
            var parameterName = (string?)attribute.GetType().GetProperty(nameof(FromAttribute<>.Name))?.GetValue(attribute)
                    ?? parameter.Name
                    ?? throw new InvalidOperationException("parameter name missing");

            resolver = new LookupResolver<TContext>(parameterName, (self, context) =>
            {
                var feature = context.Features.Where(f => f.Key.Name == featureTypeName).FirstOrDefault().Value;

                if (feature is IParameterResolverSource parameterSource)
                {
                    if (parameterSource.TryGetParameterValue(parameterName, out var value))
                    {
                        return value;
                    }
                    else
                    {
                        throw new ArgumentException($"parameter '{parameterName}' not found in feature '{featureTypeName}' via IParameterResolverSource");
                    }
                }

                return null;
            });
        }

        return resolver is not null;
    }
}