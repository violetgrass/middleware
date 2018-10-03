using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace VioletGrass.Middleware.Test
{
    public class TestMiddleware : IMiddleware
    {
        private readonly List<string> _list;

        public TestMiddleware(List<string> list)
        {
            this._list = list;
        }

        public Task InvokeAsync(Context context, MiddlewareDelegate next)
        {
            _list.Add(nameof(TestMiddleware));

            return next(context);
        }
    }
    public class IMiddlewareBuilderExtensionsTest
    {
        [Fact]
        public async Task IMiddlewareBuilderExtensions_Use_Lambda()
        {
            // arrange
            var builder = new MiddlewareBuilder();
            var list = new List<string>();

            // act
            builder.Use(async (context, next) => { list.Add("C"); await next(context); });

            var executeAsync = builder.Build();
            var x = new Context();
            await executeAsync(x);

            // assert
            Assert.Collection(list,
                l => Assert.Equal("C", l)
            );
        }

        [Fact]
        public async Task IMiddlewareBuilderExtensions_Use_IMiddleware()
        {
            // arrange
            var builder = new MiddlewareBuilder();
            var list = new List<string>();

            // act
            builder.Use(new TestMiddleware(list));

            var executeAsync = builder.Build();
            var x = new Context();
            await executeAsync(x);

            // assert
            Assert.Collection(list,
                l => Assert.Equal("TestMiddleware", l)
            );
        }
    }
}
