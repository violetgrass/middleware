using System;
using System.Reflection;

using Violet.Middleware.Handler;

namespace Violet.Middleware.Endpoints;

internal static class ControllerEndpoint
{
    public const string ControllerRouteData = "controller";
    public const string ActionRouteData = "action";

    public static void MapController<TContext, TController>(IEndpointRouteBuilder<TContext> endpointRouteBuilder, Func<TController> instanceFactory) where TContext : Context
    {
        if (endpointRouteBuilder is null)
        {
            throw new ArgumentNullException(nameof(endpointRouteBuilder));
        }

        if (instanceFactory is null)
        {
            throw new ArgumentNullException(nameof(instanceFactory));
        }

        var instanceType = typeof(TController);
        var controllerName = instanceType.Name;

        foreach (var methodInfo in instanceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            MapMethodInfo(endpointRouteBuilder, instanceFactory, controllerName, methodInfo);
        }
    }

    public static IEndpointBuilder<TContext> MapAction<TContext, TController>(IEndpointRouteBuilder<TContext> endpointRouteBuilder, Func<TController> instanceFactory, string methodName) where TContext : Context
    {
        if (endpointRouteBuilder is null)
        {
            throw new ArgumentNullException(nameof(endpointRouteBuilder));
        }

        if (instanceFactory is null)
        {
            throw new ArgumentNullException(nameof(instanceFactory));
        }

        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentException("method name missing", nameof(methodName));
        }

        var instanceType = typeof(TController);
        var controllerName = string.Empty;
        var methodInfo = instanceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

        if (methodInfo is null)
        {
            throw new InvalidOperationException("cannot find method in controller");
        }

        return MapMethodInfo(endpointRouteBuilder, instanceFactory, controllerName, methodInfo);
    }

    private static IEndpointBuilder<TContext> MapMethodInfo<TContext, TController>(IEndpointRouteBuilder<TContext> endpointRouteBuilder, Func<TController> instanceFactory, string controllerName, MethodInfo methodInfo) where TContext : Context
    {
        if (endpointRouteBuilder is null)
        {
            throw new ArgumentNullException(nameof(endpointRouteBuilder));
        }
        if (methodInfo is null)
        {
            throw new ArgumentNullException(nameof(methodInfo));
        }

        var actionName = methodInfo.Name;

        endpointRouteBuilder.PushPredicateContext(MatchControllerAction(controllerName, actionName));
        var endpointBuilder = endpointRouteBuilder.Map()
            .WithDisplayName($"{controllerName}.{actionName}")
            .WithMiddlewareDelegate(MiddlewareDelegateFactory.CreateWithInstanceFactory<TContext>(() => instanceFactory(), methodInfo, new MiddlewareDelegateOptions<TContext>()));
        endpointRouteBuilder.PopPredicateContext();

        return endpointBuilder;
    }

    private static Predicate<Context> MatchControllerAction(string controllerName, string actionName)
    {
        if (string.IsNullOrWhiteSpace(controllerName))
        {
            return context =>
            {
                var routeData = context.Feature<RouteData>();

                return routeData.Match(ActionRouteData, actionName);
            };
        }
        else
        {
            return context =>
            {
                var routeData = context.Feature<RouteData>();

                return routeData.Match(ControllerRouteData, controllerName) && routeData.Match(ActionRouteData, actionName);
            };
        }
    }
}