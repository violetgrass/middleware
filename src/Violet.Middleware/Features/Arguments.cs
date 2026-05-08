using System.Collections.Generic;

namespace Violet.Middleware.Features;

public class Arguments
{
    private readonly Dictionary<string, object> _arguments = new Dictionary<string, object>();
    public Arguments With(string name, object value)
    {
        _arguments[name] = value;

        return this;
    }

    public bool TryGetValue(string name, out object value)
        => _arguments.TryGetValue(name, out value);
}