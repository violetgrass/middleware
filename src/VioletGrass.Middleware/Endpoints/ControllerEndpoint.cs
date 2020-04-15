using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using VioletGrass.Middleware.Features;
using VioletGrass.Middleware.Router;

namespace VioletGrass.Middleware.Endpoints
{
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

            var instance = instanceFactory();
            var instanceType = instance.GetType();
            var controllerName = instanceType.Name;

            foreach (var methodInfo in instanceType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                MapMethodInfo(endpointRouteBuilder, instance, controllerName, methodInfo);
            }
        }

        public static void MapAction<TContext, TController>(IEndpointRouteBuilder<TContext> endpointRouteBuilder, Func<TController> instanceFactory, string methodName) where TContext : Context
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

            var instance = instanceFactory();
            var instanceType = instance.GetType();
            var controllerName = string.Empty;
            var methodInfo = instanceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

            MapMethodInfo(endpointRouteBuilder, instance, controllerName, methodInfo);
        }

        private static void MapMethodInfo<TContext, TController>(IEndpointRouteBuilder<TContext> endpointRouteBuilder, TController instance, string controllerName, MethodInfo methodInfo) where TContext : Context
        {
            var actionName = methodInfo.Name;
            var endpoint = new Endpoint<TContext>()
            {
                Name = $"{controllerName}.{actionName}",
                DispatcherAsync = BuildDispatcher<TContext>(instance, methodInfo),
            };

            endpointRouteBuilder.PushPredicateContext(MatchControllerAction(controllerName, actionName));
            endpointRouteBuilder.Map(endpoint);
            endpointRouteBuilder.PopPredicateContext();
        }

        private static MiddlewareDelegate<TContext> BuildDispatcher<TContext>(object instance, MethodInfo methodInfo) where TContext : Context
        {
            if (methodInfo is null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }
            // TODO: dispatch void Foo(TContext context)
            // TODO: dispatch void Foo()
            // TODO: dispatch void Foo(params) from Arguments
            return async context =>
            {
                var arguments = context.Features.Get<Arguments>();

                if (TryBuildParameters(methodInfo, arguments, out var methodInput))
                {
                    var result = methodInfo.Invoke(instance, methodInput) as Task;

                    await result;
                }
                else
                {
                    throw new InvalidOperationException("Could fill all method parameters");
                }
            };
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
            };
        }

        private static bool TryBuildParameters(MethodInfo methodInfo, Arguments argumentsFromContext, out object[] inputForMethod)
        {
            var result = new List<object>();
            bool success = true;

            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                object argumentValue = null;

                if (
                    argumentsFromContext != null &&
                    argumentsFromContext.TryGetValue(parameterInfo.Name, out var fromContext) &&
                    fromContext.GetType() == parameterInfo.ParameterType)
                {
                    argumentValue = fromContext;
                }
                else
                {
                    success = false;
                }

                result.Add(argumentValue);
            }

            inputForMethod = result.ToArray();

            return success;
        }
    }
}