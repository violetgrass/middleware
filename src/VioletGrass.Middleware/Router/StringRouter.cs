using System;
using System.Linq;
using System.Text.RegularExpressions;

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

        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateMiddlewareFactoryForRoutingKeyExtractor<TContext>(Func<TContext, string> routingKeySelector) where TContext : Context
        {
            if (routingKeySelector == null)
            {
                throw new ArgumentNullException(nameof(routingKeySelector));
            }

            return next =>
            {
                return async context =>
                {
                    var routingKey = routingKeySelector(context);

                    var routeData = context.Features.Get<RouteData>() ?? context.Features.Set(new RouteData());

                    routeData.OriginalRoutingKey = routingKey;

                    await next(context);
                };
            };
        }

        public static Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>> CreateMiddlewareFactoryForRouteDataExtractor<TContext>(string[] routePatterns) where TContext : Context
        {
            if (routePatterns == null)
            {
                throw new ArgumentNullException(nameof(routePatterns));
            }

            return next =>
            {
                var regexPatterns = routePatterns.Select(routePattern => new Regex(routePattern, RegexOptions.Compiled)).ToArray();

                return async (TContext context) =>
                {
                    var routeData = context.Features.Get<RouteData>() ?? context.Features.Set(new RouteData());

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

                    await next(context);
                };
            };
        }
    }
}