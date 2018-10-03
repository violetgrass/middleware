using System;
using System.Collections.Generic;

namespace VioletGrass.Middleware.Router
{
    public class RouteData
    {
        private Dictionary<string, string> _data = new Dictionary<string, string>();

        public string OriginalRoutingKey { get; set; }
        public bool Match(string key, string expected)
            => _data.TryGetValue(key, out var actual) && actual == expected;

        public void Add(string groupName, string value)
        {
            _data[groupName] = value;
        }
    }
}