using System;

namespace Violet.Middleware;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class FromArgumentAttribute : Attribute
{
    public FromArgumentAttribute() { }
    public FromArgumentAttribute(string name)
    {
        Name = name;
    }
    public string? Name { get; init; }
}
