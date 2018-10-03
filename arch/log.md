# Architecture Log

## 2018-10-03 Motivation

- Implementing receiving endpoints for message queues (message to function mapping) is a repetetive task. While not complex, there is no standardized middleware in .NET for message receivers.
- Dispatching of HTTP request is handled in the ASP.NET Core project in a flexible middleware, routing and function dispatching system. ASP.NET Core however is bound to Http (e.g. naming the central *context* `HttpContext`).
- Vision: Having a middleware stack available which can be bound to message queues and other event emitting systems without being bound to a technology.

## 2018-10-03 Understanding Middleware and Middleware Builders

Core

- A middleware stack is a **function** which receives a *context* (containing the input/output) (see `MiddlewareDelegate`).
- For a middleware, the rest of the stack looks like a **function** which receive the modified *context* (see `MiddlewareDelegate`)..
- A middleware builder is a **function** which receives a middleware (representing the remaining stack to call *next*) and emits a middleware to call. This builder function should integrate its own logic before and after invoking the *next* function.
- Like this, the executing function is free of the building overhead. Some functions might be lambdas carrying state.
- The primary interface is the method `IMiddlewareBuilder.Use(Func<MiddlewareDelegate, MiddlewareDelegate> middlewareFactory)` method. All other convenience `IMiddlewareBuilder.Use(..)` methods are extensions methods.

Convenience

- The core stack will provide `.Use(Func<Context, MiddlewareDelegate, Task> middleware)` extension to write simple middlewares in a lambda or function receiving *context* and *next* as parameters.
- The core stack will provide a `.Use(IMiddleware middlware)` extension to add a class implementing `IMiddleware`. This is an object oriented interface with its only method `InvokeAsync` receiving *context* and *next* as parameters.

# 2018-10-03 Understanding Middleware Branching (aka Routing)

Core

- This product considers all invokable *endpoints* as a final middleware. There is no secondary infrastructure (like in ASP.NET Core the MVC Routing/Controller/Action concepts).
- A middleware stack needs to branch to dispatch messages to multiple *endpoints*. This process is called *routing*.
- The central router method is `.UseRoute(Predicate<Context> isApplicable, Action<IMiddlewareBuilder> middlewareBuilderForRoute)`. The branches entry condition is defined in the isApplicable predicate and the branch middleware stack is built by the provided builder.

Convenience

- 