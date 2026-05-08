namespace Violet.Middleware;

public class OtherContext : Context
{
    public OtherContext(string foo)
    {
        Foo = foo;
    }

    public string Foo { get; }
}