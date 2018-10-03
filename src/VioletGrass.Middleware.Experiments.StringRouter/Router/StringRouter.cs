using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace VioletGrass.Middleware.Router
{
    public class StringRouter
    {
        public static Predicate<Context> Match(string key, string value)
        {
            return context =>
            {
                var routeData = context.Features.Get<RouteData>();

                return routeData.Match(key, value);
            };
        }

        public static Func<MiddlewareDelegate, MiddlewareDelegate> CreateMiddlewareFactoryForRoutingKeyExtractor(Func<Context, string> routingKeySelector)
        {
            return next =>
            {
                return async context =>
                {
                    var routingKey = routingKeySelector(context);

                    var routeData = context.Features.Set(new RouteData());

                    routeData.OriginalRoutingKey = routingKey;

                    await next(context);
                };
            };
        }

        public static Func<MiddlewareDelegate, MiddlewareDelegate> CreateMiddlewareFactoryForRouteDataExtractor(string[] routePatterns)
        {
            return next =>
            {
                var regexPatterns = routePatterns.Select(routePattern => new Regex(routePattern, RegexOptions.Compiled)).ToArray();

                return async context =>
                {
                    var routeData = context.Features.Get<RouteData>();

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