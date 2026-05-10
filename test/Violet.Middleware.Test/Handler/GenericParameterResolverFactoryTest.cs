using System;
using System.Threading.Tasks;

using Violet.Middleware.Features;

using Xunit;

namespace Violet.Middleware.Handler;

public class GenericParameterResolverFactoryTest
{
    [Fact]
    public async Task GenericParameterResolverFactory_ArgumentExistsExplicit()
    {
        // arrange
        var context = new Context();
        context.Features.Set(new Arguments()).With("arg1", 42);
        var options = new MiddlewareDelegateOptions<Context>()
        {
            ParameterResolverFactories =
            {
                new GenericParameterResolverFactory<Context>(),
            }
        };
        var counter = 0;

        // act
        var del = MiddlewareDelegateFactory.Create<Context>(async ([From<Arguments>("arg1")] int arg1) => { Assert.Equal(42, arg1); counter++; }, options);
        await del.Invoke(context);

        // assert
        Assert.Equal(1, counter);
    }
}