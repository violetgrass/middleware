using System.Threading.Tasks;

namespace Violet.Middleware;

public delegate Task MiddlewareDelegate<TContext>(TContext context) where TContext : Context;