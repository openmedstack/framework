# OpenMedStack.Web.Autofac.Testing

This package defines some utility types for setting up web applications for testing. It uses a `TestHost` to host the web application.

## Usage

```csharp
var webChasis = Chassis.From(configuration)
    .UsingTestWebServer(services => services.AddMvc(), app => app.UseEndpoints())
    .Build();
webChasis.Start();
```
