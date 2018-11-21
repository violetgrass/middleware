using System;

namespace VioletGrass.Middleware
{
    public interface IMiddlewareBuilder<TContext> where TContext : Context
    {
        IMiddlewareBuilder<TContext> Use(Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> middlewareBuilder);

        IMiddlewareBuilder<TContext> New();

        MiddlewareDelegate<TContext> Build();
    }
}