using System.Threading.Tasks;
using Xunit;

namespace VioletGrass.Middleware.Router
{
    public class StringRouterTest
    {
        [Fact]
        public async Task IMiddlewareBuilder_UseRoutingKey_Simple()
        {
            // arrange
            var tracer = new Tracer();
            var middleware = new MiddlewareBuilder<Context>()
                .Use(tracer.TestMiddleware("A"))
                .UseRoutingKey(c => c.Features.Get<string>())
                .UseRoutes(
                    new Route<Context>(StringRouter.ByRoutingKey("xyz.create"), b => b.Use(tracer.TestMiddleware("B"))),
                    new Route<Context>(StringRouter.ByRoutingKey("xyz.delete"), b => b.Use(tracer.TestMiddleware("C")))
                )
                .Build();

            // act
            var context = new Context();
            context.Features.Set("xyz.delete");
            await middleware(context);

            // assert
            Assert.Equal("AC", tracer.Trace);
        }

        [Fact]
        public async Task IMiddlewareBuilder_UseRoutingDataExtractor_Simple()
        {
            // arrange
            var tracer = new Tracer();
            var middleware = new MiddlewareBuilder<Context>()
                .Use(tracer.TestMiddleware("A"))
                .UseRoutingKey(c => c.Features.Get<string>(),
                    "^(?<area>.*)-home-(?<action>.*)$",
                    "^xyz\\.(?<action>.*)$"
                )
                .UseRoutes(
                    new Route<Context>(StringRouter.Match("action", "create"), b => b.Use(tracer.TestMiddleware("B"))),
                    new Route<Context>(StringRouter.Match("action", "delete"), b => b.Use(tracer.TestMiddleware("C")))
                )
                .Build();

            // act
            var context = new Context();
            context.Features.Set("xyz.delete");
            await middleware(context);

            // assert
            Assert.Equal("AC", tracer.Trace);
        }
    }
}