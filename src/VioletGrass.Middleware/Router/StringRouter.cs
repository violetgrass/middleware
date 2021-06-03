using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VioletGrass.Middleware.Router
{
    public static class StringRouter
    {
        public static Predicate<Context> Match(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("message", nameof(key));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("message", nameof(value));
            }

            return context =>
            {
                var routeData = context.Features.Get<RouteData>();

                return routeData != null && routeData.Match(key, value);
            };
        }

        public static Predicate<Context> ByRoutingKey(string routingKey)
        {
            if (string.IsNullOrWhiteSpace(routingKey))
            {
                throw new ArgumentException("message", nameof(routingKey));
            }

            return context =>
            {
                var routeData = context.Features.Get<RouteData>();

                return routeData != null && routeData.OriginalRoutingKey == routingKey;
            };
        }

        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateRoutingKeyMiddlewareFactory<TContext>(Func<TContext, string> routingKeySelector, string[] routePatterns = null) where TContext : Context
        {
            if (routingKeySelector == null)
            {
                throw new ArgumentNullException(nameof(routingKeySelector));
            }

            Regex[] regexPatterns = Array.Empty<Regex>();

            if (routePatterns != null)
            {
                regexPatterns = routePatterns.Select(routePattern => new Regex(routePattern, RegexOptions.Compiled)).ToArray();
            }

            return RoutingKeyMiddlewareFactory;

            MiddlewareDelegate<TContext> RoutingKeyMiddlewareFactory(MiddlewareDelegate<TContext> next)
            {
                return RoutingKeyMiddleware;

                async Task RoutingKeyMiddleware(TContext context)
                {
                    var routeData = context.Features.Get<RouteData>() ?? context.Features.Set(new RouteData());

                    ExtractRoutingKey(context, routingKeySelector, routeData);

                    ExtractRouteDataByRegex(regexPatterns, routeData);

                    await next(context);
                }
            };
        }

        private static void ExtractRoutingKey<TContext>(TContext context, Func<TContext, string> routingKeySelector, RouteData routeData) where TContext : Context
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (routeData is null)
            {
                throw new ArgumentNullException(nameof(routeData));
            }

            var routingKey = routingKeySelector(context);

            routeData.OriginalRoutingKey = routingKey;
        }

        private static void ExtractRouteDataByRegex(Regex[] regexPatterns, RouteData routeData)
        {
            if (regexPatterns is null)
            {
                throw new ArgumentNullException(nameof(regexPatterns));
            }

            if (routeData is null)
            {
                throw new ArgumentNullException(nameof(routeData));
            }

            foreach (var routePattern in regexPatterns)
            {
                var match = routePattern.Match(routeData.OriginalRoutingKey);

                if (match.Success)
                {
                    foreach (var groupName in routePattern.GetGroupNames())
                    {
                        routeData.Add(groupName, match.Groups[groupName].Value);
                    }

                    break;
                }
            }
        }
    }
}