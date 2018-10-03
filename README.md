# Universal Middleware Pipeline

VioletGrass Middleware is a universal middleware pipeline intended to be added to custom adapter solutions (e.g. message queue client library to business logic function).

[![Build Status](https://dev.azure.com/violetgrass/middleware/_apis/build/status/violetgrass.middleware)](https://dev.azure.com/violetgrass/middleware/_build/latest?definitionId=1)

## Examples

Creation of a simple middleware

````csharp
var executeAsync = new MiddlewareBuilder()
    .Use(async (context, next) => {
        Console.Write("Violet"); 
        await next(context); 
    })
    .Use(async (context, next) => {
        Console.Write("Grass"); 
        await next(context); 
    })
    .Build();

var x = new Context();

await executeAsync(x); // writes "VioletGrass"
````

## Goals

- Focused on a overhead free execution pipeline
- Stay generic (otherwise ASP.NET Core pipelines would be the choice)

## License

MIT