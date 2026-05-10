using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Violet.Middleware.Handler;

public abstract class FeatureBaseParameterResolverFactory<TContext, TFeature, TAttribute> : IParameterResolverFactory<TContext>
    where TContext : Context
    where TFeature : class
    where TAttribute : Attribute
{
    protected virtual object? ResolveByAttribute(ParameterInfo parameter, TAttribute attribute, TFeature feature)
        => throw new NotImplementedException();

    public bool TryGetResolver(ParameterInfo parameter, MiddlewareDelegateOptions<TContext> options, [NotNullWhen(true)] out IParameterResolver<TContext>? resolver)
    {
        resolver = null;
        string parameterName = parameter.Name ?? throw new InvalidOperationException("Parameter must have a name.");

        // Explicit, with [TArgument] attribute
        if (parameter.GetCustomAttribute(typeof(TAttribute)) is TAttribute attribute)
        {
            resolver = new LookupResolver<TContext>(parameterName, (self, context) =>
            {
                if (context.Features.Get<TFeature>() is var feature)
                {
                    return ResolveByAttribute(parameter, attribute, feature);
                }
                return null;
            });
        }

        return resolver is not null;
    }
}
