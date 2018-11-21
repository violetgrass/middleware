using System;
using System.Text;
using System.Threading.Tasks;
using VioletGrass.Middleware.Features;
using VioletGrass.Middleware.Router;
using Xunit;

namespace VioletGrass.Middleware
{
    public class MethodInfoEndpointTest
    {
        [Fact]
        public void IMiddlewareBuilder_UseMethodEndpoint_Null()
        {
            // arrange
            var builder = new MiddlewareBuilder<Context>();

            // act & assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                builder.UseMethodEndpoint(null, null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                builder.UseMethodEndpoint(typeof(IMiddlewareBuilderExtensionsTest), null);
            });
        }

        private class TestEndpoint
        {
            private StringBuilder _builder = new StringBuilder();
            public string Trace => _builder.ToString();
            public Task Foo(string a, string b)
            {
                _builder.Append(a);
                _builder.Append(b);

                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task IMiddlewareBuilder_UseMethodEndpoint_Simple()
        {
            // arrange
            var instance = new TestEndpoint();
            var middleware = new MiddlewareBuilder<Context>()
                .UseMethodEndpoint(instance, "Foo")
                .Build();

            // act
            var context = new Context();
            context.Features.Set(new Arguments()
                .With("a", "A")
                .With("b", "B"));
            await middleware(context);

            // assert
            Assert.Equal("AB", instance.Trace);
        }

        [Fact]
        public async Task IMiddlewareBuilder_UseMethodEndpoint_Full()
        {
            // arrange
            var instance = new TraceableEndpoint();
            var middleware = new MiddlewareBuilder<Context>()
                .UseRoutingKey(ctx => ctx.Features.Get<Message>().RoutingKey)
                .UseRoutingDataExtractor("^game-(?<action>.*)$")
                .UseRoutes(
                    new Route<Context>(StringRouter.Match("action", "create"), branchBuilder => branchBuilder
                        .UseJsonSerializer<Demo, Context>(ctx => ctx.Features.Get<Message>().Body, "message")
                        .UseMethodEndpoint(instance, nameof(instance.ProcessMessage))
                    )
                )
                .Build();

            // act
            var context = new Context();
            context.Features.Set(new Message("game-create", "{ \"A\": \"a\", \"B\": \"b\" }"));
            await middleware(context);

            // assert
            Assert.NotNull(instance.Message);
            Assert.Equal("a", instance.Message.A);
            Assert.Equal("b", instance.Message.B);
        }
    }
}