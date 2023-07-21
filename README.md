# OpenMedStack

OpenMedStack is a project to build a cloud-based platform for medical research and health care.
It is based on open source software and open standards.
The platform is designed to be flexible and extensible, so that it can be adapted to the needs of different use cases.

It structures the code base into a hexagonal architecture, with a core domain model and a set of adapters for different use cases.

## Getting Started

The core concept of an application is a `Chassis`.
A `Chassis` is a collection of `Modules` that are loaded into a service which is started by the `Chassis`.
The following code shows the creation of a `Chassis` with a single `Module`:

```csharp
var service = Chassis.From(
                new DeploymentConfiguration
                {
                    TenantPrefix = "test",
                    QueueName = "test",
                    ServiceBus = new Uri("loopback://localhost"),
                    Services = new Dictionary<Regex, Uri>
                    {
                        { new Regex(".+"), new Uri("loopback://localhost/test") }
                    }
                })
            .DefinedIn(GetType().Assembly)
            .AddAutofacModules((c, _) => new TestModule(c))
            .UsingNEventStore()
            .UsingInMemoryEventDispatcher(TimeSpan.FromSeconds(0.25))
            .UsingInMemoryEventStore()
            .UsingInMemoryMassTransit()
            .Build();
        service.Start();
```

The `DeploymentConfiguration` defines a type safe configuration of the service.

The `DefinedIn` method defines the assembly that contains the domain event handling definitions.

The `AddAutofacModules` method adds the `TestModule` to the `Chassis`. This is an example of adding custom Autofac modules to the service configuration.

For event storage OpenMedStack uses a customised version of NEventStore. The `UsingNEventStore` method configures the `Chassis` to use this event store.

For event dispatching OpenMedStack uses MassTransit. The `UsingInMemoryMassTransit` method configures the `Chassis` to use this event dispatcher.
