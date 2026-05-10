namespace Violet.Middleware.Handler;

public interface IParameterResolver<TContext> where TContext : Context
{
    string ParameterName { get; }
    object? Resolve(TContext context);
}
