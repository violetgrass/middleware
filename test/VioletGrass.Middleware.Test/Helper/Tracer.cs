using System;
using System.Text;

namespace VioletGrass.Middleware
{
    public class Tracer
    {
        private StringBuilder _trace = new StringBuilder();

        public Func<MiddlewareDelegate<Context>, MiddlewareDelegate<Context>> TestMiddleware(string before)
            => TestMiddleware(before, string.Empty);

        public Func<MiddlewareDelegate<Context>, MiddlewareDelegate<Context>> TestMiddleware(string before, string after)
        {
            return next => { return async context => { _trace.Append(before); await next(context); _trace.Append(after); }; };
        }

        public string Trace => _trace.ToString();
    }
}