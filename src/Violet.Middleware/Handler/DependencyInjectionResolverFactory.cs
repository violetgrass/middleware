using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Violet.Middleware.Handler;

public class DependencyInjectionResolverFactory<TContext> : IParameterResolverFactory<TContext> where TContext : Context
{
    public bool TryGetResolver(ParameterInfo parameter, MiddlewareDelegateOptions<TContext> options, [NotNullWhen(returnValue: true)] out IParameterResolver<TContext>? resolver)
    {
        if (options.ServiceProvider == null)
        {
            resolver = null;
            return false;
        }

        var service = options.ServiceProvider.GetService(parameter.ParameterType);

        if (service is not null)
        {
            resolver = new StaticValueResolver<TContext>(
                parameter.Name ?? throw new ArgumentException("Parameter must have a name.", nameof(parameter)),
                service
            );

            return true;
        }
        else
        {
            resolver = null;
            return false;
        }
    }
}
