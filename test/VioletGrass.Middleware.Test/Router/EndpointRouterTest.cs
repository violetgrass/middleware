using System;
using System.Threading.Tasks;
using Xunit;

namespace VioletGrass.Middleware.Router
{
    public class EndpointRouterTest
    {
        [Fact]
        public async Task IMiddlewareBuilder_UseRouting_NoUse()
        {
            // arrange
            var path = new Tracer();
            Endpoint<Context> selectedEndpoint = null;
            var stack = new MiddlewareBuilder<Context>()
                .Use(path.TestMiddleware("A"))
                .UseRouting()
                .Use(path.TestMiddleware("B"))
                .Use(async (context, next) =>
                {
                    selectedEndpoint = context.Feature<EndpointRoutingFeature<Context>>().Endpoint;

                    await next(context);
                })
                .Use(path.TestMiddleware("C"))
                .Build();

            // act
            await stack(new Context());

            // assert
            // no exception till here
            Assert.Null(selectedEndpoint);
            Assert.Equal("ABC", path.Trace);
        }


        [Fact]
        public async Task IMiddlewareBuilder_UseRouting_NoBranchingEndpoint()
        {
            // arrange
            var path = new Tracer();
            Endpoint<Context> selectedEndpoint = null;
            var stack = new MiddlewareBuilder<Context>()
                .Use(path.TestMiddleware("A"))
                .UseRouting()
                .Use(path.TestMiddleware("B"))
                .Use(async (context, next) =>
                {
                    context.Feature<EndpointRoutingFeature<Context>>().TryEvaluate(context);
                    selectedEndpoint = context.Feature<EndpointRoutingFeature<Context>>().Endpoint;

                    await next(context);
                })
                .Use(path.TestMiddleware("C"))
                .UseEndpoint(endpointBuilder =>
                {
                    endpointBuilder.MapLambda("", context => { });
                })
                .Build();

            // act
            await stack(new Context());

            // assert
            // no exception till here
            Assert.NotNull(selectedEndpoint);
            Assert.Equal("ABC", path.Trace);
        }

        [Theory]
        [InlineData("ABCD", true, false, false, "D_Endpoint")]
        [InlineData("ABCD", true, true, false, "D_Endpoint")] // first one wins
        [InlineData("ABCE", false, true, false, "E_Endpoint")]
        [InlineData("ABCF", false, true, true, "F_Endpoint")]
        [InlineData("ABC", false, false, true, null)] // none selected (even with sub tree eval)
        public async Task IMiddlewareBuilder_UseRouting_MultipleEndpoint(string expectedTrace, bool routeDPredicate, bool routeEPredicate, bool routeFPredicate, string expectedEndpointName)
        {
            // arrange
            var path = new Tracer();
            string actualInvokedEndpoint = null;
            Endpoint<Context> selectedEndpoint = null;
            var stack = new MiddlewareBuilder<Context>()
                .Use(path.TestMiddleware("A"))
                .UseRouting()
                .Use(path.TestMiddleware("B"))
                .Use(async (context, next) =>
                {
                    context.Feature<EndpointRoutingFeature<Context>>().TryEvaluate(context);
                    selectedEndpoint = context.Feature<EndpointRoutingFeature<Context>>().Endpoint;

                    await next(context);
                })
                .Use(path.TestMiddleware("C"))
                .UseRoutes(
                    new Route<Context>(context => routeDPredicate, branchMiddlewareBuilder => branchMiddlewareBuilder
                        .Use(path.TestMiddleware("D"))
                        .UseEndpoint(endpointBuilder => endpointBuilder.MapLambda("D_Endpoint", context => { actualInvokedEndpoint = "D_Endpoint"; }))
                    ),
                    new Route<Context>(context => routeEPredicate, branchMiddlewareBuilder => branchMiddlewareBuilder
                        .UseRoutes(
                            new Route<Context>(context => !routeFPredicate, branchMiddlewareBuilder => branchMiddlewareBuilder
                                .Use(path.TestMiddleware("E"))
                                .UseEndpoint(endpointBuilder => endpointBuilder.MapLambda("E_Endpoint", context => { actualInvokedEndpoint = "E_Endpoint"; }))
                            ),
                            new Route<Context>(context => routeFPredicate, branchMiddlewareBuilder => branchMiddlewareBuilder
                                .Use(path.TestMiddleware("F"))
                                .UseEndpoint(endpointBuilder => endpointBuilder.MapLambda("F_Endpoint", context => { actualInvokedEndpoint = "F_Endpoint"; }))
                            )
                        )
                    )
                )
                .Build();

            // act
            await stack(new Context());

            // assert
            // no exception till here
            Assert.Equal(expectedEndpointName, selectedEndpoint?.Name);
            Assert.Equal(expectedTrace, path.Trace);
            Assert.Equal(expectedEndpointName, actualInvokedEndpoint);
        }
    }
}