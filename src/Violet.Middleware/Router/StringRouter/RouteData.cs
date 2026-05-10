using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Violet.Middleware.Handler;

namespace Violet.Middleware;

public class RouteData : IParameterResolverSource
{
    private readonly Dictionary<string, string> _data = new Dictionary<string, string>();

    public string? OriginalRoutingKey { get; set; }
    public bool Match(string key, string expected)
        => _data.TryGetValue(key, out var actual) && actual == expected;

    public void Add(string groupName, string value)
    {
        _data[groupName] = value;
    }

    public bool TryGetValue(string key, [NotNullWhen(returnValue: true)] out string? value)
        => _data.TryGetValue(key, out value);

    public bool TryGetParameterValue(string name, [NotNullWhen(returnValue: true)] out object? value)
    {
        if (name == nameof(OriginalRoutingKey))
        {
            value = OriginalRoutingKey;
        }
        else if (_data.TryGetValue(name, out var stringValue))
        {
            value = stringValue;
        }
        else
        {
            value = null;
        }

        return value is not null;
    }
}