using System;
using System.Collections.Generic;

namespace VioletGrass.Middleware
{
    public interface IMiddlewareBuilder<TContext> where TContext : Context
    {
        IMiddlewareBuilder<TContext> Use(Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> middlewareBuilder);

        IDictionary<string, object> Properties { get; }

        IMiddlewareBuilder<TContext> New();

        MiddlewareDelegate<TContext> Build();
    }
}