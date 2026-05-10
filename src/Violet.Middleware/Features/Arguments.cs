using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Violet.Middleware.Handler;

namespace Violet.Middleware.Features;

public class Arguments : IParameterResolverSource
{
    private readonly Dictionary<string, object> _arguments = new Dictionary<string, object>();
    public Arguments With(string name, object value)
    {
        _arguments[name] = value;

        return this;
    }

    public bool TryGetValue(string name, [NotNullWhen(returnValue: true)] out object? value)
        => _arguments.TryGetValue(name, out value);

    public bool TryGetParameterValue(string name, [NotNullWhen(true)] out object? value)
        => _arguments.TryGetValue(name, out value);
}