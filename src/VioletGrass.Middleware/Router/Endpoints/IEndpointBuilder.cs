using System;
using System.Collections.Generic;

namespace VioletGrass.Middleware
{
    public interface IEndpointBuilder<TContext> where TContext : Context
    {
        MiddlewareDelegate<TContext> MiddlewareDelegate { get; set; }
        IList<object> Metadata { get; }
        string DisplayName { get; set; }
        IServiceProvider ServiceProvider { get; }
        IList<Predicate<TContext>> Predicates { get; }

        Endpoint<TContext> Build();
    }
}