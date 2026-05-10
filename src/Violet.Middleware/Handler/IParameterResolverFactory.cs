using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Violet.Middleware.Handler;

public interface IParameterResolverFactory<TContext> where TContext : Context
{
    bool TryGetResolver(ParameterInfo parameter, MiddlewareDelegateOptions<TContext> options, [NotNullWhen(returnValue: true)] out IParameterResolver<TContext>? resolver);
}
