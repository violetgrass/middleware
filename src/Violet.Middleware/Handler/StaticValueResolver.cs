namespace Violet.Middleware.Handler;

public class StaticValueResolver<TContext> : IParameterResolver<TContext> where TContext : Context
{
    public string ParameterName { get; }
    public object? Value { get; }

    public StaticValueResolver(string parameterName, object? value)
    {
        ParameterName = parameterName;
        Value = value;
    }

    public object? Resolve(TContext context) => Value;
}
