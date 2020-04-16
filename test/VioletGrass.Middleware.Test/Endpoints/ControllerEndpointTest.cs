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
        private class TestController
        {
            private StringBuilder _builder = new StringBuilder();
            public string Trace => _builder.ToString();
            public Task Foo(string a, string b)
            {
                _builder.Append(a);
                _builder.Append(b);

                return Task.CompletedTask;
            }

            public Task Fuba()
            {
                _builder.Append(nameof(Fuba));

                return Task.CompletedTask;
            }
            public Task Foobar()
            {
                _builder.Append(nameof(Foobar));

                return Task.CompletedTask;
            }
            public Task Bar()
            {
                _builder.Append(nameof(Bar));

                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task IEndpointRouteBuilder_MapControllerAction_Simple()
        {
            // arrange
            var instance = new TestController();
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
        public async Task IEndpointRouteBuilder_MapControllerAction_Full()
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

        [Theory]
        [InlineData("TestController/Fuba", "Fuba")]
        [InlineData("TestController/Foobar", "Foobar")]
        [InlineData("TestController/Bar", "Bar")]
        [InlineData("TestController", "")]
        [InlineData("TestController/zzz", "")]
        [InlineData("xyz/Bar", "")]
        [InlineData("/Bar", "")]
        [InlineData("", "")]
        public async Task IEndpointRouteBuilder_MapController_Full(string routingKey, string expected)
        {
            // arrange
            var controller = new TestController();

            var stack = new MiddlewareBuilder<Context>()
                .UseRouting()
                .UseRoutingKey(c => c.Feature<string>(), @"^(?<controller>.*)/(?<action>.*)$")
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapController(controller);
                })
                .Build();

            // act
            await stack(new Context(routingKey));

            // assert
            Assert.Equal(expected, controller.Trace);
        }
    }
}