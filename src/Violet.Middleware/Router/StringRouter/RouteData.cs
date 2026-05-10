using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Violet.Middleware;

public class RouteData
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
}