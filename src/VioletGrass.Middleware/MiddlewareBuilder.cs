using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VioletGrass.Middleware
{

    public class MiddlewareBuilder<TContext> : IMiddlewareBuilder<TContext> where TContext : Context
    {
        private List<Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>>> _factories = new List<Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>>>();

        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public IServiceProvider ServiceProvider { get; }

        public MiddlewareBuilder(IServiceProvider serviceProvider = default)
        {
            ServiceProvider = serviceProvider;
        }

        public IMiddlewareBuilder<TContext> Use(Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> middlewareFactory)
        {
            _factories.Add(middlewareFactory);

            return this;
        }

        public MiddlewareDelegate<TContext> Build()
        {
            _factories.Reverse();

            MiddlewareDelegate<TContext> current = TerminalMiddleware; // safeguard

            foreach (var middlewareFactory in _factories)
            {
                current = middlewareFactory(current);
            }

            return current;
        }

        private Task TerminalMiddleware(Context context)
            => Task.CompletedTask;

        public IMiddlewareBuilder<TContext> New()
            => new MiddlewareBuilder<TContext>(ServiceProvider);
    }
}