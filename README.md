# Universal Middleware Pipeline

VioletGrass Middleware is a universal middleware pipeline intended to be added to custom adapter solutions (e.g. message queue client library to business logic function).

[![Build Status](https://dev.azure.com/violetgrass/middleware/_apis/build/status/violetgrass.middleware)](https://dev.azure.com/violetgrass/middleware/_build/latest?definitionId=1)

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

## Experimental StringRouter

While the core engine does support routes, it does on programmatic predicates and not on a string matching. The `StringRouter` adds support for a HTTP Url like matching of routing keys (e.g. from queing systems).

See the unit tests for it.

## Goals

- Focused on a overhead free execution pipeline
- Stay generic (otherwise ASP.NET Core pipelines would be the choice)
- Use Case 1: Dispatch incoming events to processing functions (e.g. from message queue)
- Use Case 2: Extension Points in Software

## License

MIT