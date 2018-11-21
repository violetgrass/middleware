using System.Threading.Tasks;
using VioletGrass.Middleware.Features;
using Xunit;

namespace VioletGrass.Middleware
{
    public partial class IMiddlewareBuilderExtensionsTest
    {
        private class TestEndpoint2
        {
            public Demo Message { get; private set; }

            public Task Foo(Demo message)
            {
                Message = message;
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task IMiddlewareBuilder_UseJsonSerializer_Simple()
        {
            // arrange
            var instance = new TestEndpoint2();
            var stack = new MiddlewareBuilder<Context>()
                .UseJsonSerializer<Demo, Context>(ctx => ctx.Features.Get<string>(), "message")
                .UseMethodEndpoint(instance, "Foo")
                .Build();

            // act
            var context = new Context();
            context.Features.Set("{ \"A\": \"a\", \"B\": \"b\" }");

            await stack(context);

            // assert
            Assert.NotNull(instance.Message);
            Assert.Equal("a", instance.Message.A);
            Assert.Equal("b", instance.Message.B);
        }
    }
}