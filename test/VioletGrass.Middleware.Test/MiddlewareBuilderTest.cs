using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace VioletGrass.Middleware.Test
{
    public class MiddlewareBuilderTest
    {

        [Fact]
        public async Task MiddlewareBuilder_Build_Empty()
        {
            // arrange
            var builder = new MiddlewareBuilder<Context>();

            // act
            var executeAsync = builder.Build();
            var ctx = new Context();
            await executeAsync(ctx);

            // assert

            // .. no exception till here.
        }

        [Fact]
        public async Task MiddlewareBuilder_Build_ForNullContext()
        {
            // arrange
            var builder = new MiddlewareBuilder<Context>();

            // act
            builder.Use(next => { return async context => { await next(context); }; });

            var executeAsync = builder.Build();
            await executeAsync(null);

            // assert

            // .. no exception till here.
        }

        [Fact]
        public async Task MiddlewareBuilder_Use_LambdaFactory()
        {
            // arrange
            var builder = new MiddlewareBuilder<Context>();
            var list = new List<string>();

            // act
            builder.Use(next => { return async context => { list.Add("C"); await next(context); }; });

            var executeAsync = builder.Build();
            var x = new Context();
            await executeAsync(x);

            // assert
            Assert.Collection(list,
                l => Assert.Equal("C", l)
            );
        }

        [Fact]
        public async Task MiddlewareBuilder_New_DoesNotReuseUseCalls()
        {
            // arrange
            IMiddlewareBuilder<Context> builder = new MiddlewareBuilder<Context>();
            var list = new List<string>();
            builder.Use(next => { return async context => { list.Add("D"); await next(context); }; });

            // act
            var newBuilder = builder.New();
            newBuilder.Use(next => { return async context => { list.Add("C"); await next(context); }; });

            var executeAsync = newBuilder.Build();
            var x = new Context();
            await executeAsync(x);

            // assert
            Assert.Collection(list,
                l => Assert.Equal("C", l)
            );
        }

        [Fact]
        public async Task MiddlewareBuilder_Use_LinearOrder()
        {
            // arrange
            var builder = new MiddlewareBuilder<Context>();
            var list = new List<string>();

            // act
            builder.Use(next => { return async context => { list.Add("A"); await next(context); list.Add("Z"); }; });
            builder.Use(next => { return async context => { list.Add("B"); await next(context); list.Add("Y"); }; });
            builder.Use(next => { return async context => { list.Add("C"); await next(context); list.Add("X"); }; });

            var executeAsync = builder.Build();
            var x = new Context();
            await executeAsync(x);

            // assert
            Assert.Collection(list,
                l => Assert.Equal("A", l),
                l => Assert.Equal("B", l),
                l => Assert.Equal("C", l),
                l => Assert.Equal("X", l),
                l => Assert.Equal("Y", l),
                l => Assert.Equal("Z", l)
            );
        }

        [Fact]
        public async Task MiddlewareBuilder_Use_WithEarlyTerminal()
        {
            // arrange
            var builder = new MiddlewareBuilder<Context>();
            var list = new List<string>();

            // act
            builder.Use(next => { return async context => { list.Add("A"); await next(context); list.Add("Z"); }; });
            builder.Use(next => { return context => { list.Add("B"); list.Add("Y"); return Task.CompletedTask; }; });
            builder.Use(next => { return async context => { list.Add("C"); await next(context); list.Add("X"); }; });

            var executeAsync = builder.Build();
            var x = new Context();
            await executeAsync(x);

            // assert
            Assert.Collection(list,
                l => Assert.Equal("A", l),
                l => Assert.Equal("B", l),
                l => Assert.Equal("Y", l),
                l => Assert.Equal("Z", l)
            );
        }
    }
}
