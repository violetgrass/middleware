using System.Threading.Tasks;

namespace Violet.Middleware;

public interface IMiddleware<TContext> where TContext : Context
{
    Task InvokeAsync(TContext context, MiddlewareDelegate<TContext> next);
}