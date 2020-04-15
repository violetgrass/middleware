using System;
using System.Threading.Tasks;

namespace VioletGrass.Middleware
{
    public class Endpoint<TContext> where TContext : Context
    {
        public string Name { get; set; }

        public MiddlewareDelegate<TContext> DispatcherAsync { get; set; }
    }
}