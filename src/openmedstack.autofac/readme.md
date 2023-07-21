# OpenMedStack.Autofac

This package defines the core implementations of the Autofac based dependency injection.

## Usage

`AutofacService` is the default implementation of the `IService` interface. It uses Autofac to resolve dependencies.

```csharp
var autofacChasis = Chassis.From(configuration)
    .AddAutofacModules((config, assemblies) => new MyAutofacModule(config, assemblies))
    .Build();
autofacChasis.Start();
```
