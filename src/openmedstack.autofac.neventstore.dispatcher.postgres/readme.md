# OpenMedStack.Autofac.MassTransit.NEventStore

This package defines the core implementations of the OpenMedStack NEventStore integration for event sourced storage.

Use together with the `OpenMedStack.Autofac.MassTransit.NEventStore.InMemory` package for in-memory event storage, or
`OpenMedStack.Autofac.MassTransit.NEventStore.Sql` for database storage.

## Usage

```csharp
var neChasis = Chassis.From(configuration)
    .UsingNEventStore()
    .UsingInMemoryEventDispatcher()
    [Select storage]    
    .Build();
neChasis.Start();
```
