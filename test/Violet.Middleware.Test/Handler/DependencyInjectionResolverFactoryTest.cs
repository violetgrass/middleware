using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Violet.Middleware.Features;

using Xunit;

namespace Violet.Middleware.Handler;

public class DependencyInjectionResolverFactoryTest
{
    [Fact]
    public async Task DependencyInjectionResolverFactory_Resolve_Exists()
    {
        // arrange
        var sp = new ServiceCollection()
            .AddSingleton<string>("Hello World")
            .BuildServiceProvider();

        var context = new Context();
        context.Features.Set(new Arguments()).With("arg1", 42);
        var options = new MiddlewareDelegateOptions<Context>()
        {
            ServiceProvider = sp,
            ParameterResolverFactories =
            {
                new DependencyInjectionResolverFactory<Context>(),
            }
        };
        var counter = 0;

        // act
        var del = MiddlewareDelegateFactory.Create<Context>(async (string arg1) => { Assert.Equal("Hello World", arg1); counter++; }, options);
        await del.Invoke(context);

        // assert
        Assert.Equal(1, counter);
    }
    [Fact]
    public async Task DependencyInjectionResolverFactory_Resolve_DoesNotExists()
    {
        // arrange
        var sp = new ServiceCollection()
            .BuildServiceProvider();

        var context = new Context();
        var options = new MiddlewareDelegateOptions<Context>()
        {
            ServiceProvider = sp,
            ParameterResolverFactories =
            {
                new DependencyInjectionResolverFactory<Context>(),
            }
        };

        // act
        // assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var del = MiddlewareDelegateFactory.Create<Context>(async (string arg1) => { }, options);
        });
    }

    [Fact]
    public async Task DependencyInjectionResolverFactory_Resolve_WithOthers()
    {
        // arrange
        var sp = new ServiceCollection()
            .AddSingleton<DependencyInjectionResolverFactoryTest>(new DependencyInjectionResolverFactoryTest())
            .BuildServiceProvider();

        var context = new Context();
        context.Features.Set(new Arguments()).With("arg1", "Hello World");
        var options = new MiddlewareDelegateOptions<Context>()
        {
            ServiceProvider = sp,
            ParameterResolverFactories =
            {
                new ArgumentsParameterResolverFactory<Context>(),
                new DependencyInjectionResolverFactory<Context>(),
            }
        };
        var counter = 0;

        // act
        var del = MiddlewareDelegateFactory.Create<Context>(async ([Argument] string arg1, DependencyInjectionResolverFactoryTest x) => { Assert.Equal("Hello World", arg1); Assert.NotNull(x); counter++; }, options);
        await del.Invoke(context);

        // assert
        Assert.Equal(1, counter);
    }
}