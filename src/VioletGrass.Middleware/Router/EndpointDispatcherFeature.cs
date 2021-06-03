using System;

namespace VioletGrass.Middleware.Router
{
    internal partial class EndpointRouter
    {
        internal class EndpointDispatcherFeature
        {
            public Guid? DispatcherId { get; set; } = null;
        }
    }
}