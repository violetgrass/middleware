using System;
using System.Threading.Tasks;
using VioletGrass.Middleware.Features;

namespace VioletGrass.Middleware
{
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
}
