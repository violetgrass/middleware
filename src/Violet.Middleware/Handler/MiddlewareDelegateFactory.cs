using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Violet.Middleware.Handler;

public class MiddlewareDelegateOptions<TContext> where TContext : Context
{
    public IList<IParameterResolverFactory<TContext>> ParameterResolverFactories { get; init; } = new List<IParameterResolverFactory<TContext>>()
    {
        new GenericParameterResolverFactory<TContext>(),
        new DependencyInjectionResolverFactory<TContext>(),
    };
    public IServiceProvider? ServiceProvider { get; init; }
}

public static class MiddlewareDelegateFactory
{
    public static MiddlewareDelegate<TContext> Create<TContext>(Delegate handler, MiddlewareDelegateOptions<TContext> options) where TContext : Context
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        var method = handler.GetMethodInfo();
        var target = handler.Target;

        return Create<TContext>(target, method, options);
    }
    public static MiddlewareDelegate<TContext> Create<TContext>(object? instance, MethodInfo handler, MiddlewareDelegateOptions<TContext> options) where TContext : Context
        => CreateWithInstanceFactory<TContext>(() => instance, handler, options);
    public static MiddlewareDelegate<TContext> CreateWithInstanceFactory<TContext>(Func<object?> instanceFactory, MethodInfo handler, MiddlewareDelegateOptions<TContext> options) where TContext : Context
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));
        if (options == null) throw new ArgumentNullException(nameof(options));

        // TODO shortcut this already in the delegate call
        if (handler.ReturnParameter.ParameterType == typeof(Task) &&
            handler.GetParameters() is [{ ParameterType: TContext }])
        {
            return async context =>
            {
                var task = (Task)handler.Invoke(null, new object[] { context })!;
                await task;
            };
        }

        var parameterResolvers = new List<IParameterResolver<TContext>>();

        // analyze handler at build time
        foreach (var parameter in handler.GetParameters())
        {
            IParameterResolver<TContext>? resolver = null;
            foreach (var factory in options.ParameterResolverFactories)
            {
                if (factory.TryGetResolver(parameter, options, out resolver))
                {
                    break;
                }
            }

            if (resolver == null)
            {
                throw new InvalidOperationException($"No resolver found for parameter '{parameter.Name}' of type '{parameter.ParameterType.FullName}' in method '{handler.DeclaringType?.FullName}.{handler.Name}'.");
            }

            parameterResolvers.Add(resolver);
        }

        return async context =>
        {
            var parameters = new object?[parameterResolvers.Count];
            for (int i = 0; i < parameterResolvers.Count; i++)
            {
                parameters[i] = parameterResolvers[i].Resolve(context);
            }

            object? result;

            if (handler.IsStatic)
            {
                result = handler.Invoke(null, parameters);
            }
            else
            {
                var instance = instanceFactory();

                result = handler.Invoke(instance, parameters);
            }

            if (result is Task taskResult)
            {
                await taskResult;
            }
        };
    }

}