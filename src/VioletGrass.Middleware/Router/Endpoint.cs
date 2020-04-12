using System;
using System.Threading.Tasks;

namespace VioletGrass.Middleware
{
    public class Endpoint<TContext>
    {
        public string Name { get; set; }

        public Func<TContext, Task> DispatcherAsync { get; set; }
    }
}