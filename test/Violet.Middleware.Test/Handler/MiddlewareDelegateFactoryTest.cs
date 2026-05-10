using System.Threading.Tasks;

using Xunit;

namespace Violet.Middleware.Handler;

public static class StaticTestHandler
{
    public static int MiddlewareDelegateFactory_Create_StaticTargetStaticFunction = 0;

    public static Task StaticFunction()
    {
        MiddlewareDelegateFactory_Create_StaticTargetStaticFunction++;
        return Task.CompletedTask;
    }
}
public class MiddlewareDelegateFactoryTest
{
    private static Task StaticFunction()
    {
        MiddlewareDelegateFactory_Create_InstanceTargetStaticFunctionCounter++;
        return Task.CompletedTask;
    }
    private Task InstanceFunction()
    {
        MiddlewareDelegateFactory_Create_InstanceTargetInstanceFunctionCounter++;
        return Task.CompletedTask;
    }
    public static int MiddlewareDelegateFactory_Create_InstanceTargetStaticFunctionCounter = 0;
    public int MiddlewareDelegateFactory_Create_InstanceTargetInstanceFunctionCounter = 0;
    public static int MiddlewareDelegateFactory_Create_StaticAnonymousFunctionCounter = 0;

    [Fact]
    public async Task MiddlewareDelegateFactory_Create_StaticTargetStaticFunction()
    {
        // arrange
        var context = new Context();

        // act
        var del = MiddlewareDelegateFactory.Create<Context>(StaticTestHandler.StaticFunction, new MiddlewareDelegateOptions<Context>());
        await del.Invoke(context);

        // assert
        Assert.Equal(1, StaticTestHandler.MiddlewareDelegateFactory_Create_StaticTargetStaticFunction);
    }
    [Fact]
    public async Task MiddlewareDelegateFactory_Create_InstanceTargetStaticFunction()
    {
        // arrange
        var context = new Context();

        // act
        var del = MiddlewareDelegateFactory.Create<Context>(StaticFunction, new MiddlewareDelegateOptions<Context>());
        await del.Invoke(context);

        // assert
        Assert.Equal(1, MiddlewareDelegateFactory_Create_InstanceTargetStaticFunctionCounter);
    }
    [Fact]
    public async Task MiddlewareDelegateFactory_Create_StaticAnonymousFunction()
    {
        // arrange
        var context = new Context();

        // act
        var del = MiddlewareDelegateFactory.Create<Context>(static () => { MiddlewareDelegateFactory_Create_StaticAnonymousFunctionCounter++; return Task.CompletedTask; }, new MiddlewareDelegateOptions<Context>());
        await del.Invoke(context);

        // assert
        Assert.Equal(1, MiddlewareDelegateFactory_Create_StaticAnonymousFunctionCounter);
    }
    [Fact]
    public async Task MiddlewareDelegateFactory_Create_InstanceTargetInstanceFunction()
    {
        // arrange
        var context = new Context();

        // act
        var del = MiddlewareDelegateFactory.Create<Context>(InstanceFunction, new MiddlewareDelegateOptions<Context>());
        await del.Invoke(context);

        // assert
        Assert.Equal(1, MiddlewareDelegateFactory_Create_InstanceTargetInstanceFunctionCounter);
    }
    [Fact]
    public async Task MiddlewareDelegateFactory_Create_InstanceAnonymousFunction()
    {
        // arrange
        var context = new Context();
        var counter = 0;

        // act
        var del = MiddlewareDelegateFactory.Create<Context>(() => { counter++; return Task.CompletedTask; }, new MiddlewareDelegateOptions<Context>());
        await del.Invoke(context);

        // assert
        Assert.Equal(1, counter);
    }

    [Fact]
    public async Task MiddlewareDelegateFactory_Create_InstanceAnonymousFunctionWithVoidResult()
    {
        // arrange
        var context = new Context();
        var counter = 0;

        // act
        var del = MiddlewareDelegateFactory.Create<Context>(() => { counter++; }, new MiddlewareDelegateOptions<Context>());
        await del.Invoke(context);

        // assert
        Assert.Equal(1, counter);
    }
}