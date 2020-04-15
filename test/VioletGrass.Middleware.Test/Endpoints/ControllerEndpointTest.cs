using System;
using System.Text;
using System.Threading.Tasks;
using VioletGrass.Middleware.Features;
using VioletGrass.Middleware.Router;
using Xunit;

namespace VioletGrass.Middleware
{
    public class ControllerEndpointTest
    {
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
        public async Task IEndpointRouteBuilder_UseControllerAction_Simple()
        {
            // arrange
            var instance = new TestEndpoint();
            var stack = new MiddlewareBuilder<Context>()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerAction(instance, "Foo");
                })
                .Build();

            // act
            var context = new Context();
            context.Features.Set(new Arguments()
                .With("a", "A")
                .With("b", "B"));
            var routeData = new RouteData();
            routeData.Add("action", "Foo");
            context.Features.Set(routeData);
            await stack(context);

            // assert
            Assert.Equal("AB", instance.Trace);
        }

        [Fact]
        public async Task IEndpointRouteBuilder_UseControllerAction_Full()
        {
            // arrange
            var instance = new TraceableEndpoint();
            var stack = new MiddlewareBuilder<Context>()
                .UseRouting()
                .UseRoutingKey(context => context.Feature<Message>().RoutingKey, "^game-(?<action>.*)$")
                .UseJsonSerializer<Demo, Context>(ctx => ctx.Features.Get<Message>().Body, "message")
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerAction(instance, nameof(instance.ProcessMessage));
                })
                .Build();

            // act
            var context = new Context();
            context.Features.Set(new Message("game-ProcessMessage", "{ \"A\": \"a\", \"B\": \"b\" }"));

            await stack(context);

            // assert
            Assert.NotNull(instance.Message);
            Assert.Equal("a", instance.Message.A);
            Assert.Equal("b", instance.Message.B);
        }
    }
}