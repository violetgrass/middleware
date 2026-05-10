using System;
using System.Threading.Tasks;

using Violet.Middleware.Features;

using Xunit;

namespace Violet.Middleware.Handler;

public class ArgumentsParameterResolverFactoryTest
{
    [Fact]
    public async Task ArgumentsParameterResolverFactory_ByArgument_ArgumentExistsExplicit()
    {
        // arrange
        var context = new Context();
        context.Features.Set(new Arguments()).With("arg1", 42);
        var options = new MiddlewareDelegateOptions<Context>()
        {
            ParameterResolverFactories =
            {
                new ArgumentsParameterResolverFactory<Context>(),
            }
        };
        var counter = 0;

        // act
        var del = MiddlewareDelegateFactory.Create<Context>(async ([FromArgument("arg1")] int arg1) => { Assert.Equal(42, arg1); counter++; }, options);
        await del.Invoke(context);

        // assert
        Assert.Equal(1, counter);
    }
    [Fact]
    public async Task ArgumentsParameterResolverFactory_ByArgument_ArgumentDoesNotExistsExplicitThrows()
    {
        // arrange
        var context = new Context();
        context.Features.Set(new Arguments()).With("arg1", 42);
        var options = new MiddlewareDelegateOptions<Context>()
        {
            ParameterResolverFactories =
            {
                new ArgumentsParameterResolverFactory<Context>(),
            }
        };
        var counter = 0;

        // act
        var del = MiddlewareDelegateFactory.Create<Context>(async ([FromArgument("arg55")] int arg1) => { counter++; }, options);
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await del.Invoke(context);
        });

        // assert
        Assert.Equal(0, counter);
    }
    [Fact]
    public async Task ArgumentsParameterResolverFactory_ByArgument_TwoArgumentsExplicit()
    {
        // arrange
        var context = new Context();
        context.Features.Set(new Arguments()).With("arg1", 42).With("arg2", "blub");
        var options = new MiddlewareDelegateOptions<Context>()
        {
            ParameterResolverFactories =
            {
                new ArgumentsParameterResolverFactory<Context>(),
            }
        };
        var counter = 0;

        // act
        var del = MiddlewareDelegateFactory.Create<Context>(async ([FromArgument("arg1")] int arg1, [FromArgument("arg2")] string arg2) => { Assert.Equal(42, arg1); Assert.Equal("blub", arg2); counter++; }, options);
        await del.Invoke(context);

        // assert
        Assert.Equal(1, counter);
    }
    [Fact]
    public async Task ArgumentsParameterResolverFactory_Resolve_ByArgumentImplicit()
    {
        // arrange
        var context = new Context();
        context.Features.Set(new Arguments()).With("arg4", 42);
        var options = new MiddlewareDelegateOptions<Context>()
        {
            ParameterResolverFactories =
            {
                new ArgumentsParameterResolverFactory<Context>(),
            }
        };
        var counter = 0;

        // act
        var del = MiddlewareDelegateFactory.Create<Context>(async ([FromArgument] int arg4) => { Assert.Equal(42, arg4); counter++; }, options);
        await del.Invoke(context);

        // assert
        Assert.Equal(1, counter);
    }
}