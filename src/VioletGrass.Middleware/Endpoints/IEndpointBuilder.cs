using System.Collections.Generic;

namespace VioletGrass.Middleware
{
    public interface IEndpointBuilder<TContext> where TContext : Context
    {
        MiddlewareDelegate<TContext> MiddlewareDelegate { get; set; }
        IList<object> Metadata { get; }
        string DisplayName { get; set; }

        Endpoint<TContext> Build();
    }
}