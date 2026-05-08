using System;

namespace Violet.Middleware.Router;

internal class EndpointDispatcherScope
{
    public Guid? DispatcherId { get; set; } = null;
}