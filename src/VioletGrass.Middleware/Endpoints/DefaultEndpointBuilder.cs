using System;
using System.Collections.Generic;

namespace VioletGrass.Middleware
{
    public class DefaultEndpointBuilder<TContext> : IEndpointBuilder<TContext> where TContext : Context
    {
        public DefaultEndpointBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public MiddlewareDelegate<TContext> MiddlewareDelegate { get; set; }

        public IList<object> Metadata { get; } = new List<object>();

        public string DisplayName { get; set; }

        public IList<Predicate<TContext>> Predicates { get; } = new List<Predicate<TContext>>();

        public IServiceProvider ServiceProvider { get; }

        public Endpoint<TContext> Build()
            => new Endpoint<TContext>(DisplayName, Metadata, MiddlewareDelegate);
    }
}