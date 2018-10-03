using System.Threading.Tasks;

namespace VioletGrass.Middleware
{
    public interface IMiddleware
    {
        Task InvokeAsync(Context context, MiddlewareDelegate next);
    }
}
