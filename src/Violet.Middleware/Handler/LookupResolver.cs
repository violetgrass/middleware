using System;

namespace Violet.Middleware.Handler;

public class LookupResolver<TContext> : IParameterResolver<TContext> where TContext : Context
{
    public string ParameterName { get; }
    public Func<LookupResolver<TContext>, TContext, object?> Lookup { get; }

    public LookupResolver(string parameterName, Func<LookupResolver<TContext>, TContext, object?> lookup)
    {
        ParameterName = parameterName;
        Lookup = lookup;
    }

    public object? Resolve(TContext context) => Lookup(this, context);
}
