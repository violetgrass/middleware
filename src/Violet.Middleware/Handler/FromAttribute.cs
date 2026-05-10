using System;

namespace Violet.Middleware;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class FromAttribute<TFeature> : Attribute where TFeature : class
{
    public FromAttribute() { }
    public FromAttribute(string parameterName)
    {
        Name = parameterName;
    }
    public string? Name { get; init; }

    public Type FeatureType => typeof(TFeature);
}
