using System.Threading.Tasks;

namespace VioletGrass.Middleware
{
    public interface IMiddleware<TContext> where TContext : Context
    {
        Task InvokeAsync(TContext context, MiddlewareDelegate<TContext> next);
    }
}
