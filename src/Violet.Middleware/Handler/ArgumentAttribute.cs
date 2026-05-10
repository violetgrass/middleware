using System;

namespace Violet.Middleware;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class ArgumentAttribute : Attribute
{
    public ArgumentAttribute() { }
    public ArgumentAttribute(string name)
    {
        Name = name;
    }
    public string? Name { get; init; }
}
