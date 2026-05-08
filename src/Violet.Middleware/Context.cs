using Violet.Middleware.Features;

namespace Violet.Middleware;

public class Context
{
    public Context()
    {
        Features = new FeatureCollection();
    }
    public Context(params object[] features)
    {
        Features = new FeatureCollection(features);
    }

    public FeatureCollection Features { get; }
}