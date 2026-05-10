using System.Diagnostics.CodeAnalysis;

namespace Violet.Middleware.Handler;

public interface IParameterResolverSource
{
    bool TryGetParameterValue(string name, [NotNullWhen(returnValue: true)] out object? value);
}
