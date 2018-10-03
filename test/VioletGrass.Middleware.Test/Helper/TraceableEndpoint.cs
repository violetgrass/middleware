using System.Threading.Tasks;

namespace VioletGrass.Middleware
{
    public class TraceableEndpoint
    {
        public Demo Message { get; private set; }

        public Task ProcessMessage(Demo message)
        {
            Message = message;
            return Task.CompletedTask;
        }
    }
}