using System.Threading.Tasks;

namespace VioletGrass.Middleware
{
    public delegate Task MiddlewareDelegate<TContext>(TContext context) where TContext : Context;
}