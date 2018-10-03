using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace VioletGrass.Middleware.Router
{
    public class IMiddlewareBuilderExtensionsTest
    {
        [Fact]
        public async Task IMiddlewareBuilderExtensions_UseRoute_Empty()
        {
            // arrange
            var builder = new MiddlewareBuilder();
            var path = new Tracer();

            builder.Use(path.TestMiddleware("A"));

            builder.UseRoutes();

            builder.Use(path.TestMiddleware("B"));

            var stack = builder.Build();

            // act
            await stack(new Context());

            // assert
            // no exception till here
            Assert.Equal("AB", path.Trace);
        }

        [Fact]
        public async Task IMiddlewareBuilderExtensions_UseRoute_OneBranchMet()
        {
            // arrange
            var builder = new MiddlewareBuilder();
            var path = new Tracer();

            builder.Use(path.TestMiddleware("A"));

            builder.UseRoutes(new Route(context => true, branchBuilder =>
            {
                branchBuilder.Use(path.TestMiddleware("C"));
            }
            ));

            builder.Use(path.TestMiddleware("B"));

            var stack = builder.Build();

            // act
            await stack(new Context());

            // assert
            // no exception till here
            Assert.Equal("AC", path.Trace);
        }

        [Fact]
        public async Task IMiddlewareBuilderExtensions_UseRoute_OneBranchMetMultipleMiddleware()
        {
            // arrange
            var builder = new MiddlewareBuilder();
            var path = new Tracer();

            builder.Use(path.TestMiddleware("A"));

            builder.UseRoutes(new Route(context => true, branchBuilder =>
            {
                branchBuilder.Use(path.TestMiddleware("C"));
                branchBuilder.Use(path.TestMiddleware("D"));
            }
            ));

            builder.Use(path.TestMiddleware("B"));

            var stack = builder.Build();

            // act
            await stack(new Context());

            // assert
            // no exception till here
            Assert.Equal("ACD", path.Trace);
        }

        [Fact]
        public async Task IMiddlewareBuilderExtensions_UseRoute_OneBranchNotMet()
        {
            // arrange
            var builder = new MiddlewareBuilder();
            var path = new Tracer();

            builder.Use(path.TestMiddleware("A"));

            builder.UseRoutes(new Route(context => false, branchBuilder =>
            {
                branchBuilder.Use(path.TestMiddleware("C"));
            }
            ));

            builder.Use(path.TestMiddleware("B"));

            var stack = builder.Build();

            // act
            await stack(new Context());

            // assert
            // no exception till here
            Assert.Equal("AB", path.Trace);
        }

        [Fact]
        public async Task IMiddlewareBuilderExtensions_UseRoute_FirstBranchTaken()
        {
            // arrange
            var builder = new MiddlewareBuilder();
            var path = new Tracer();

            builder.Use(path.TestMiddleware("A"));

            builder.UseRoutes(
            new Route(context => true, branchBuilder =>
                {
                    branchBuilder.Use(path.TestMiddleware("C"));
                }
            ),
            new Route(context => true, branchBuilder =>
                {
                    branchBuilder.Use(path.TestMiddleware("D"));
                }
            ));

            builder.Use(path.TestMiddleware("B"));

            var stack = builder.Build();

            // act
            await stack(new Context());

            // assert
            // no exception till here
            Assert.Equal("AC", path.Trace);
        }
        [Fact]
        public async Task IMiddlewareBuilderExtensions_UseRoute_FirstBranchSkipped()
        {
            // arrange
            var builder = new MiddlewareBuilder();
            var path = new Tracer();

            builder.Use(path.TestMiddleware("A"));

            builder.UseRoutes(
            new Route(context => false, branchBuilder =>
                {
                    branchBuilder.Use(path.TestMiddleware("C"));
                }
            ),
            new Route(context => true, branchBuilder =>
                {
                    branchBuilder.Use(path.TestMiddleware("D"));
                }
            ));

            builder.Use(path.TestMiddleware("B"));

            var stack = builder.Build();

            // act
            await stack(new Context());

            // assert
            // no exception till here
            Assert.Equal("AD", path.Trace);
        }
    }
}