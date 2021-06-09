using System.Collections.Generic;

namespace VioletGrass.Middleware
{
    public class Endpoint<TContext> where TContext : Context
    {
        public Endpoint(string displayName, IList<object> metadata, MiddlewareDelegate<TContext> middlewareDelegate)
        {
            DisplayName = displayName;
            Metadata = metadata;
            MiddlewareDelegate = middlewareDelegate;
        }

        public string DisplayName { get; set; }
        public IList<object> Metadata { get; }

        public MiddlewareDelegate<TContext> MiddlewareDelegate { get; set; }
    }
}