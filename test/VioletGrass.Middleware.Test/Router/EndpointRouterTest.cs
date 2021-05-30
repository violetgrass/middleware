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
                    context.Feature<EndpointFeature<Context>>().TryGetEndpoint(context, out selectedEndpoint);

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
        public async Task IMiddlewareBuilder_UseEndpoints_WithoutUseRouting()
        {
            // arrange
            var path = new Tracer();
            var stack = new MiddlewareBuilder<Context>()
                .Use(path.TestMiddleware("A"))
                .UseEndpoints(endpoints =>
                {

                })
                .Build();

            // act
            await stack(new Context());

            // assert
            // no exception till here
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
                    context.Feature<EndpointFeature<Context>>().TryGetEndpoint(context, out selectedEndpoint);

                    await next(context);
                })
                .Use(path.TestMiddleware("C"))
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapLambda("", context => { });
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
                    context.Feature<EndpointFeature<Context>>().TryGetEndpoint(context, out selectedEndpoint);

                    await next(context);
                })
                .Use(path.TestMiddleware("C"))
                .UseRoutes(
                    new Route<Context>(context => routeDPredicate, branchMiddlewareBuilder => branchMiddlewareBuilder
                        .Use(path.TestMiddleware("D"))
                        .UseEndpoints(endpoints => endpoints.MapLambda("D_Endpoint", context => { actualInvokedEndpoint = "D_Endpoint"; }))
                    ),
                    new Route<Context>(context => routeEPredicate, branchMiddlewareBuilder => branchMiddlewareBuilder
                        .UseRoutes(
                            new Route<Context>(context => !routeFPredicate, branchMiddlewareBuilder => branchMiddlewareBuilder
                                .Use(path.TestMiddleware("E"))
                                .UseEndpoints(endpoints => endpoints.MapLambda("E_Endpoint", context => { actualInvokedEndpoint = "E_Endpoint"; }))
                            ),
                            new Route<Context>(context => routeFPredicate, branchMiddlewareBuilder => branchMiddlewareBuilder
                                .Use(path.TestMiddleware("F"))
                                .UseEndpoints(endpoints => endpoints.MapLambda("F_Endpoint", context => { actualInvokedEndpoint = "F_Endpoint"; }))
                            )
                        )
                    )
                )
                .Build();

            // act
            await stack(new Context());

            // assert
            // no exception till here
            Assert.Equal(expectedEndpointName, selectedEndpoint?.DisplayName);
            Assert.Equal(expectedTrace, path.Trace);
            Assert.Equal(expectedEndpointName, actualInvokedEndpoint);
        }


        [Fact]
        public async Task IEndpointRouteBuilder_MapLambda_EndpointBuilderRequires()
        {
            // arrange
            string result = "";

            var stack = new MiddlewareBuilder<Context>()
                .UseRouting()
                .UseRoutingKey(c => c.Feature<string>(), @"^(?<xyz>.*)/(?<action>.*)$")
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapLambda("X", _ => result = "X")
                        .Requires(StringRouter.Match("action", "X"));
                    endpoints.MapLambda("Y", _ => result = "Y")
                        .Requires(StringRouter.Match("action", "Y"));
                })
                .Build();

            // act
            await stack(new Context("x/Y"));

            // assert
            Assert.Equal("Y", result);
        }
    }
}