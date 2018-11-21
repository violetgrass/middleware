using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace VioletGrass.Middleware
{
    public class TestMiddleware<TContext> : IMiddleware<TContext> where TContext : Context
    {
        private readonly List<string> _list;

        public TestMiddleware(List<string> list)
        {
            this._list = list;
        }

        public Task InvokeAsync(TContext context, MiddlewareDelegate<TContext> next)
        {
            _list.Add(nameof(TestMiddleware<TContext>));

            return next(context);
        }
    }
    public partial class IMiddlewareBuilderExtensionsTest
    {
        [Fact]
        public async Task IMiddlewareBuilderExtensions_Use_Lambda()
        {
            // arrange
            var builder = new MiddlewareBuilder<Context>();
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
            var builder = new MiddlewareBuilder<Context>();
            var list = new List<string>();

            // act
            builder.Use(new TestMiddleware<Context>(list));

            var executeAsync = builder.Build();
            var x = new Context();
            await executeAsync(x);

            // assert
            Assert.Collection(list,
                l => Assert.Equal("TestMiddleware", l)
            );
        }

        [Fact]
        public async Task IMiddlewareBuilderExtensions_Use_OtherContext()
        {
            // arrange
            var builder = new MiddlewareBuilder<OtherContext>();
            var list = new List<string>();

            // act
            builder.Use(next => { return async context => { list.Add(context.GetType().Name + " B " + context.Foo); await next(context); }; });
            builder.Use(new TestMiddleware<OtherContext>(list));
            builder.Use(async (context, next) => { list.Add(context.GetType().Name + " C " + context.Foo); await next(context); });

            var executeAsync = builder.Build();
            var x = new OtherContext("bar");
            await executeAsync(x);

            // assert
            Assert.Collection(list,
                l => Assert.Equal("OtherContext B bar", l),
                l => Assert.Equal("TestMiddleware", l),
                l => Assert.Equal("OtherContext C bar", l)
            );
        }
    }
}
