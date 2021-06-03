using System.Threading.Tasks;
using Xunit;

namespace VioletGrass.Middleware
{
    public class DefaultEndpointBuilderTest
    {
        [Fact]
        public void DefaultEndpointBuilder_Build_Empty()
        {
            // arange
            var endpoint = new DefaultEndpointBuilder<Context>(null)
            // act
                .Build();

            // assert
            Assert.Null(endpoint.MiddlewareDelegate);
            Assert.Null(endpoint.DisplayName);
            Assert.Empty(endpoint.Metadata);
        }

        [Fact]
        public void DefaultEndpointBuilder_Build_WithDisplayName()
        {
            // arange
            var endpoint = new DefaultEndpointBuilder<Context>(null)
            // act
                .WithDisplayName("Foo")
                .Build();

            // assert
            Assert.Null(endpoint.MiddlewareDelegate);
            Assert.Equal("Foo", endpoint.DisplayName);
            Assert.Empty(endpoint.Metadata);
        }

        [Fact]
        public void DefaultEndpointBuilder_Build_WithMiddlwareDelegate()
        {
            // arange
            MiddlewareDelegate<Context> x = (context) => Task.CompletedTask;

            var endpoint = new DefaultEndpointBuilder<Context>(null)
            // act
                .WithMiddlewareDelegate(x)
                .Build();

            // assert
            Assert.NotNull(endpoint.MiddlewareDelegate);
            Assert.Same(x, endpoint.MiddlewareDelegate);
            Assert.Null(endpoint.DisplayName);
            Assert.Empty(endpoint.Metadata);
        }

        [Fact]
        public void DefaultEndpointBuilder_Build_WithMetadata()
        {
            // arange
            var endpoint = new DefaultEndpointBuilder<Context>(null)
            // act
                .WithMetadata("Hello World")
                .Build();

            // assert
            Assert.Null(endpoint.MiddlewareDelegate);
            Assert.Null(endpoint.DisplayName);
            Assert.Collection(endpoint.Metadata,
                m => { Assert.Equal("Hello World", m); }
            );
        }
    }
}