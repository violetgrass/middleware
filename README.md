# Universal Middleware Pipeline

VioletGrass.Middleware is a universal middleware pipeline intended to be added to custom adapter solutions (e.g. message queue client library to business logic function) or extension points within applications.

![build](https://github.com/violetgrass/middleware/workflows/Build-CI/badge.svg)
![license:MIT](https://img.shields.io/github/license/violetgrass/middleware?style=flat-square)
[![Nuget](https://img.shields.io/nuget/v/VioletGrass.Middleware?style=flat-square)](https://www.nuget.org/packages/VioletGrass.Middleware/)

## Features

- **Extensible Middleware Builder**: A middleware builder building a composite function based on a three staged phase (configuration, building, actual middleware). Extensible by extension methods with access to a IServiceBuilder.
- **Parameterizable Context Type**: The key capability not offered by the ASP.NET Core Abstractions. The core invocation context is a parameterizable type (e.g. `MessageQueueContext`, `ImageProcessorContext`, ...).
- **Middleware Branching**: A middleware can be branched and designed for different middleware routes. The route matching is based on predicates (and not URLs) and can be customized for specific need. A simple regular expression based `StringRouter` is included.
- **Endpoint Routing**: Endpoints can be defined with routing predicates and metadata attached to them, which is accessible througout the middleware stack.

## Examples

All example should work under namespace `using VioletGrass.Middleware` (after installing NuGet package `VioletGrass.Middleware`)

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

Public APIs: `IMiddlewareBuilder<TContext>`, `MiddlewareBuilder<TContext>`, `MiddlewareDelegate` (build result), `IMiddlewareBuilderExtensions` (`Use` with lambda), `Context` (base class).

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

Public APIs: `Context` (base class), `TContext` (type parameters on most of library)

### Predicate based routing (usally behind the scenes)

Middleware can branch. The basic switching pattern is a predicate utilizing the context. Each branch creates a new middleware.

````csharp
var stack = new MiddlewareBuilder<Context>()
    .UseRoutes(
        new Route<Context>(context => context.Feature<string>() == "Hello", branchBuilder => branchBuilder
            .Use(async (context, next) => { Console.Write("Hello"); await next(context); })
            .Use(async (context, next) => { Console.Write("World"); await next(context); })
        ),
        new Route<Context>(context => true, branchBuilder => branchBuilder
            .Use(async (context, next) => { Console.Write("I am never called"); await next(context); })
        )
    )
    .Build();

// for the sake of simplicity, misusing string as a feature
await stack(new Context("Hello"));
````

Public APIs: `Route<TContext>`, `IMiddlewareBuilderExtensions` (UseRoutes)

### String Router

While the core engine does support routes, it does on programmatic predicates and not on a string matching. The `StringRouter` adds support for routing keys (e.g. from queing systems or the HTTP Uri based on regular expressions).

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

Public APIs: `IMiddlewareBuilderExtensions` (UseRoutingKey), `StringRouter` (for matching), `RouteData` (behind the scenes)

### Endpoint Routing

Endpoint Routing enables earliers middlewares to understand the routing and the endpoints of later middlewares configuration. The can access the final endpoint (if the necessary data is present to evaluate the route predicates and additional predicates pushed to the endpoints). Built on top of Predicate based Routing, in example here used with String Router.

````csharp
var stack = new MiddlewareBuilder<Context>()
    .UseRouting() // enables endpoint routing
    .UseRoutingKey(context => context.Feature<string>(), "^(?<action>.*)$") // has to be extracted ASAP (without route data no branch evaluation can be done)
    .Use(async (context, next) => {
        context.Feature<EndpointFeature>().TryGetEndpoint(context, out var endpoint); // evaluate branches and determine endpoint
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
            .UseEndpoints(endpoints => {
                endpoints.MapLambda("Foo", async () => Console.WriteLine("Hello World")); // endpoint is constrained by "action = Hello"
            })
        ),
        new Route<Context>(StringRouter.Match("action", "Foo"), branchBuilder => branchBuilder
            .Use(async (context, next) => { Console.Write("I am never called"); await next(context); })
            .UseEndpoints(endpoints => {
                endpoints.MapLambda("Bar", async (context) => Console.WriteLine("Never World")); // endpoint is constrained by "action = Foo"
            })
        )
    )
    .Build();

// for the sake of simplicity, misusing string as a feature
await stack(new Context("Hello"));
````

Each `UseEndpoints` can contain multiple mapped endpoints which can be annotated by additional predicates. `UseEndpoints` which do not having a matching endpoint in it are invoking the next middleware. If they have an applicable endpoint, they execute the endpoint.

````csharp
var stack = new MiddlewareBuilder<Context>()
    .UseRouting()
    .UseRoutingKey(c => c.Feature<string>(), @"^action-(?<action>.*)$")
    .UseEndpoints(endpoints =>
    {
        // map a lambda and require a match with action route data
        endpoints.MapLambda("my-lambda-a", _ => { /* ... */ })
            .Requires(StringRouter.Match("action", "A"));

        endpoints.MapLambda("my-lambda-a", _ => { /* ... */ })
            .Requires(StringRouter.Match("action", "B"));
    })
    .Use(c => { /* only invoked if action is different than A or B */ })
    .Build();

await stack(new Context("action-B"));
````

Public APIs: `IMiddlewareBuilderExtensions` (UseRouting and UseEndpoints), `IEndpointRouteBuilder`, `IEndpointBuilder<TContext>`, `Endpoint<TContext>` `IEndpointRouteBuilderExtensions` (MapLambda), `EndpointFeature<TContext>` (TryGetEndpoint)

### 🏃‍♂️ Experimental ControllerEndpoint

***Note**: This concept is a work in progress. The interface is not stable and may change on minor releases*

The ControllerEndpoint is a endpoint builder extension which allow dispatching a stack against a regular .NET class. Explicit middleware branching is not required as the endpoints can internally push predicates (like controller/action names) before mapping an endpoint.

````csharp
var controller = new TestController(); // singleton, has multiple endpoints as functions

var stack = new MiddlewareBuilder<Context>()
    .UseRouting()
    .UseRoutingKey(c => c.Feature<string>(), @"^(?<controller>.*)/(?<action>.*)$")
    .UseEndpoints(endpoints =>
    {
         // map all public methods as endpoints and register them to controller/action route constraints
        endpoints.MapController(controller);
    })
    .Build();

await stack(new Context(routingKey));
````

Public APIs: `IEndpointRouteBuilderExtensions` (MapController and MapControllerAction).

## Documentation

- [Architecture Log](docs/arch-log.md)
- Inspiraton: [1](https://speakerdeck.com/davidfowl/asp-dot-net-core-architecture-overview)

## Community, Contributions, License

[Code of Conduct](CODE_OF_CONDUCT.md) (🏃‍♂️ Monitored Email Pending)

🏃‍♂️ Contributing Guideline (not yet done)

[MIT licensed](LICENSE)

---

Legend: 🏃‍♂️ In Progress, ⌛ Not Yet Available