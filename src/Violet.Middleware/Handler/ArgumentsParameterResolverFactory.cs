using System;
using System.Reflection;

using Violet.Middleware.Features;

namespace Violet.Middleware.Handler;

public class ArgumentsParameterResolverFactory<TContext> : FeatureBaseParameterResolverFactory<TContext, Arguments, ArgumentAttribute>, IParameterResolverFactory<TContext> where TContext : Context
{
    protected override object? ResolveByAttribute(ParameterInfo parameter, ArgumentAttribute attribute, Arguments feature)
    {
        var parameterName = attribute.Name ?? parameter.Name ?? string.Empty;
        if (!feature.TryGetValue(parameterName, out var value))
        {
            throw new ArgumentException($"Argument '{parameterName}' not found in Arguments of {nameof(TContext)}.");
        }

        return value;
    }
}
