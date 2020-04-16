using System;
using System.Threading.Tasks;

namespace VioletGrass.Middleware
{
    public class Endpoint<TContext> where TContext : Context
    {
        public string DisplayName { get; set; }

        public MiddlewareDelegate<TContext> MiddlewareDelegate { get; set; }
    }
}