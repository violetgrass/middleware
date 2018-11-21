using System.Threading.Tasks;

namespace VioletGrass.Middleware
{
    public interface IMiddleware<TContext> where TContext : Context
    {
        Task InvokeAsync(Context context, MiddlewareDelegate<TContext> next);
    }
}
