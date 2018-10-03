using System;

namespace VioletGrass.Middleware
{
    public interface IMiddlewareBuilder
    {
        IMiddlewareBuilder Use(Func<MiddlewareDelegate, MiddlewareDelegate> middlewareBuilder);

        IMiddlewareBuilder New();

        MiddlewareDelegate Build();
    }
}