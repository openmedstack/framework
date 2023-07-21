# OpenMedStack.Web.Autofac

This package defines some utility types for setting up web applications.

A web chassis can be used in connection with a standard chassis to set up a web application with event publication support,
where the publishers and consumers are run as background tasks.

## Usage

```csharp
var webChasis = Chassis.From(configuration)
    .UsingWebServer(services => services.AddMvc(), app => app.UseEndpoints())
    .Build();
webChasis.Start();
```
