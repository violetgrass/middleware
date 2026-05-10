using System;

namespace Violet.Middleware;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class RouteDataAttribute : Attribute
{
    public string? Name { get; init; }
}
