# Universal Middleware Pipeline

VioletGrass.Middleware is a universal middleware pipeline intended to be added to custom adapter solutions (e.g. message queue client library to business logic function).

![build](https://github.com/violetgrass/middleware/workflows/Build-CI/badge.svg)
![license:MIT](https://img.shields.io/github/license/violetgrass/middleware?style=flat-square)
![Nuget](https://img.shields.io/nuget/v/VioletGrass.Middleware?style=flat-square)

## Examples

### Creation of a simple stack only middleware

Used as core foundation, can be utilized for extensions points (it is essentially just a function in the end) or as the base for an extensive dispatching infrastructure.

````csharp
var stack = new MiddlewareBuilder<Context>()
    .Use(async (context, next) => {
        Console.Write("Violet"); 
        await next(context); 
    })
    .Use(async (context, next) => {
        Console.Write("Grass"); 
        await next(context); 
    })
    .Build();

// a context can be also other types being derived from Context
var x = new Context();

// this is just a function
await stack(x); // writes "VioletGrass"
````

### TContext Type Parameter

The purpose of this library is the flexible usage in different scenarios. ASP.NET Core's middleware stack is tightly coupled to the `HttpContext`. As a result it cannot be re-used. `IMiddlewareBuilder<TContext>` and the rest of the library is built with re-use for different context types in mind.

````csharp
public class QueueMessageContext : Context
{
    public Message Message { get; set; }
    ...
}

var stack = new MiddlewareBuilder<QueueMessageContext>()
    .Use(async (context, next) => {
        Console.Write(context.Message.Body); 
        await next(context); 
    })
    .Build();

var x = new QueueMessageContext(...);

await stack(x);
````

### Predicate based routing (usally behind the scenes)

Middleware can branch. The basic switching pattern is a predicate utilizing the context. Each branch creates a new middleware.

````csharp
var stack = new MiddlewareBuilder<Context>()
    .UseRoutes(
        new Route<Context>(context => context.Feature<string>() == "Hello", branchBuilder =>
        {
            branchBuilder
                .Use(async (context, next) => { Console.Write("Hello"); await next(context); })
                .Use(async (context, next) => { Console.Write("World"); await next(context); });
        }),
        new Route<Context>(context => true, branchBuilder =>
        {
            branchBuilder
                .Use(async (context, next) => { Console.Write("I am never called"); await next(context); });
        })
    )
    .Build();

// for the sake of simplicity, misusing string as a feature
await stack(new Context("Hello"));
````

### 🏃‍♂️ Experimental String Router

***Note**: This concept is a work in progress. The interface is not stable and may change on minor releases*

While the core engine does support routes, it does on programmatic predicates and not on a string matching. The `StringRouter` adds support for a HTTP Url like matching of routing keys (e.g. from queing systems).

The method `UseRoutingKey` selects a string from the `TContext` to be (optionally) dissected in multiple

````csharp
var stack = new MiddlewareBuilder<Context>()
    .UseRoutingKey(c => c.Feature<Message>().RoutingKey, // select a routing key from the context (e.g. a MQ routing key or the HTTP Uri)
        "^(?<area>.*)-home-(?<action>.*)$", // regex as string extraction methods
        "^xyz\\.(?<action>.*)$"
    )
    .UseRoutes(
        new Route<Context>(StringRouter.Match("action", "create"), b => b
            .Use(async (context, next) => { Console.Write($"Create {context.Feature<Message>().Body}"); await next(context); })),
        new Route<Context>(StringRouter.Match("action", "delete"), b => b
            .Use(async (context, next) => { Console.Write("Delete Hello"); await next(context); }))
    )
    .Build();

await stack(new Context(new Message("xyz.delete", "Hello World")));
````

See the [unit tests](test\VioletGrass.Middleware.Test\Router\StringRouterTest.cs) for it.

### 🏃‍♂️ Experimental Endpoint Routing

***Note**: This concept is a work in progress. The interface is not stable and may change on minor releases*

Endpoint Routing enables earliers middlewares to understand the routing and the endpoints of later middlewares configuration. The can access the final endpoint (if the necessary data is present to evaluate the route predicates and additional predicates pushed to the endpoints). Built on top of Predicate based Routing, in example here used with String Router

````csharp
var stack = new MiddlewareBuilder<Context>()
    .UseRouting() // enables endpoint routing
    .UseRoutingKey(context => context.Feature<string>(), "^(?<action>.*)$") // has to be extracted ASAP (without route data no branch evaluation can be done)
    .Use(async (context, next) => {
        context.Feature<EndpointRoutingFeature>().TryGetEndpoint(context, out var endpoint); // evaluate branches and determine endpoint
        if (endpoint?.Name == "Foo") { // not possible otherwise
            Console.WriteLine("[LOG] Before Foo Call");
            await next(context);
            Console.WriteLine("[LOG] After Foo Call");
        } else {
            await next(context);
        }
    })
    .UseRoutes(
        new Route<Context>(StringRouter.Match("action", "Hello"), branchBuilder => branchBuilder
            .Use(async (context, next) => { Console.Write("Hello"); await next(context); })
            .Use(async (context, next) => { Console.Write("World"); await next(context); })
            .UseEndpoint(endpointBuilder => {
                endpointBuilder.MapLambda("Foo", async () => Console.WriteLine("Hello World"));
            })
        ),
        new Route<Context>(StringRouter.Match("action", "Foo"), branchBuilder => branchBuilder
            .Use(async (context, next) => { Console.Write("I am never called"); await next(context); })
            .UseEndpoint(endpointBuilder => {
                endpointBuilder.MapLambda("Bar", async (context) => Console.WriteLine("Never World"));
            })
        )
    )
    .Build();

// for the sake of simplicity, misusing string as a feature
await stack(new Context("Hello"));
````

See the [unit tests](test\VioletGrass.Middleware.Test\Router\EndpointRouterTest.cs) for it.

### 🏃‍♂️ Experimental MethodInfoEndpoint

***Note**: This concept is a work in progress. The interface is not stable and may change on minor releases*

The MethodInfoEndpoint is a final middleware dispatching a stack endpoint to a regular .NET method.

See the [unit tests](test\VioletGrass.Middleware.Test\Endpoints\MethodInfoEndpointTest.cs) for it.

## Goals

- Focused on a overhead free execution pipeline
- Stay generic (otherwise ASP.NET Core pipelines would be the choice)
- Use Case 1: Dispatch incoming events to processing functions (e.g. from message queue)
- Use Case 2: Extension Points in Software

## Documentation

[Architecture Log](docs/arch-log.md)

## Community, Contributions, License

[Code of Conduct](CODE_OF_CONDUCT.md) (🏃‍♂️ Monitored Email Pending)

🏃‍♂️ Contributing Guideline (not yet done)

[MIT licensed](LICENSE)

---

Legend: 🏃‍♂️ In Progress, ⌛ Not Yet Available