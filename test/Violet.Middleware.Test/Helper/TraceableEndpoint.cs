using System.Threading.Tasks;

using Violet.Middleware.Features;

namespace Violet.Middleware;

public class TraceableEndpoint
{
    public Demo Message { get; private set; }

    public Task ProcessMessage([From<Arguments>] Demo message)
    {
        Message = message;
        return Task.CompletedTask;
    }
}