using System;

namespace Violet.Middleware;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class FromRouteDataAttribute : Attribute
{
    public string? Name { get; init; }
}
