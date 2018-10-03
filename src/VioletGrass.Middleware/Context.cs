using System;
using System.Threading.Tasks;
using VioletGrass.Middleware.Features;

namespace VioletGrass.Middleware
{
    public class Context
    {
        public FeatureCollection Features { get; } = new FeatureCollection();
    }
}
