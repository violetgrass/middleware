using System.Threading.Tasks;

namespace Violet.Middleware;

public class TraceableEndpoint
{
    public Demo Message { get; private set; }

    public Task ProcessMessage([FromArgument] Demo message)
    {
        Message = message;
        return Task.CompletedTask;
    }
}