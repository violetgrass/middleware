using System.Collections.Generic;

namespace VioletGrass.Middleware
{
    public class DefaultEndpointBuilder<TContext> : IEndpointBuilder<TContext> where TContext : Context
    {
        public MiddlewareDelegate<TContext> MiddlewareDelegate { get; set; }

        public IList<object> Metadata { get; } = new List<object>();

        public string DisplayName { get; set; }

        public Endpoint<TContext> Build()
            => new Endpoint<TContext>()
            {
                DisplayName = DisplayName,
                MiddlewareDelegate = MiddlewareDelegate
            };
    }
}