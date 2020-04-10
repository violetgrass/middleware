using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VioletGrass.Middleware.Features
{
    public class FeatureCollection : IEnumerable<KeyValuePair<Type, object>>
    {
        private Dictionary<Type, object> _features;
        public FeatureCollection()
        {
            _features = new Dictionary<Type, object>();
        }
        public FeatureCollection(object[] features)
        {
            _features = features.ToDictionary(feature => feature.GetType());
        }
        public IEnumerator<KeyValuePair<Type, object>> GetEnumerator()
            => _features.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _features.GetEnumerator();

        public TFeature Get<TFeature>() where TFeature : class
        {
            TFeature result = default;

            if (_features.TryGetValue(typeof(TFeature), out var entry))
            {
                result = entry as TFeature;
            }

            return result;
        }

        public TFeature Set<TFeature>(TFeature instance) where TFeature : class
        {
            _features[typeof(TFeature)] = instance;
            return instance;
        }
    }
}