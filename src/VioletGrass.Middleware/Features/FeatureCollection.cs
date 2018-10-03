using System;
using System.Collections;
using System.Collections.Generic;

namespace VioletGrass.Middleware.Features
{
    public class FeatureCollection : IEnumerable<KeyValuePair<Type, object>>
    {
        private Dictionary<Type, object> _features = new Dictionary<Type, object>();
        public IEnumerator<KeyValuePair<Type, object>> GetEnumerator()
            => _features.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _features.GetEnumerator();

        public TFeature Get<TFeature>() where TFeature : class
            => _features[typeof(TFeature)] as TFeature;

        public TFeature Set<TFeature>(TFeature instance) where TFeature : class
        {
            _features[typeof(TFeature)] = instance;
            return instance;
        }
    }
}