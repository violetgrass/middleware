# Universal Middleware Pipeline

VioletGrass Middleware is a universal middleware pipeline intended to be added to custom adapter solutions (e.g. message queue client library to business logic function).

![build](https://github.com/violetgrass/workitems/middleware/Build-CI/badge.svg)
![license:MIT](https://img.shields.io/github/license/violetgrass/middleware?style=flat-square)
![Nuget](https://img.shields.io/nuget/v/VioletGrass.Middleware?style=flat-square)

## Examples

Creation of a simple middleware

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

Predicate based routing

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

await stack(new Context("Hello"));
````

## 🏃‍♂️ Experimental StringRouter

***Note**: This concept is a work in progress. The interface is not stable and may change on minor releases*

While the core engine does support routes, it does on programmatic predicates and not on a string matching. The `StringRouter` adds support for a HTTP Url like matching of routing keys (e.g. from queing systems).

See the [unit tests](test\VioletGrass.Middleware.Test\Router\StringRouterTest.cs) for it.

## 🏃‍♂️ Experimental MethodInfoEndpoint

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