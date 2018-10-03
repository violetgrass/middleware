using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VioletGrass.Middleware
{

    public class MiddlewareBuilder : IMiddlewareBuilder
    {
        private List<Func<MiddlewareDelegate, MiddlewareDelegate>> _factories = new List<Func<MiddlewareDelegate, MiddlewareDelegate>>();

        public IMiddlewareBuilder Use(Func<MiddlewareDelegate, MiddlewareDelegate> middlewareFactory)
        {
            _factories.Add(middlewareFactory);

            return this;
        }

        public MiddlewareDelegate Build()
        {
            _factories.Reverse();

            MiddlewareDelegate current = TerminalMiddleware; // safeguard

            foreach (var middlewareFactory in _factories)
            {
                current = middlewareFactory(current);
            }

            return current;
        }

        private Task TerminalMiddleware(Context context)
            => Task.CompletedTask;

        public IMiddlewareBuilder New()
            => new MiddlewareBuilder();
    }
}