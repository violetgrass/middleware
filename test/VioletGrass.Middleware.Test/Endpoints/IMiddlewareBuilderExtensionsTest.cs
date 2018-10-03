using System;
using System.Text;
using System.Threading.Tasks;
using VioletGrass.Middleware.Features;
using Xunit;

namespace VioletGrass.Middleware
{
    public partial class IMiddlewareBuilderExtensionsTest
    {
        [Fact]
        public void IMiddlewareBuilder_UseMethodEndpoint_Null()
        {
            // arrange
            var builder = new MiddlewareBuilder();

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
            var middleware = new MiddlewareBuilder()
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
    }
}