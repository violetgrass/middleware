using System;
using System.Reflection;

namespace Violet.Middleware.Handler;

public class RouteDataParameterResolverFactory<TContext> : FeatureBaseParameterResolverFactory<TContext, RouteData, FromRouteDataAttribute>, IParameterResolverFactory<TContext> where TContext : Context
{
    protected override object? ResolveByAttribute(ParameterInfo parameter, FromRouteDataAttribute attribute, RouteData feature)
    {
        var parameterName = attribute.Name ?? parameter.Name ?? string.Empty;
        if (!feature.TryGetValue(parameterName, out var value))
        {
            throw new ArgumentException($"Route data '{parameterName}' not found in RouteData of {nameof(TContext)}.");
        }

        return value;
    }
}