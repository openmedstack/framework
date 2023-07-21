# OpenMedStack.Autofac.MassTransit.NEventStore.InMemory

This package defines the core implementations of InMemory storage for the OpenMedStack NEventStore.

This package is useful for testing and local development.

## Usage

```csharp
var neChasis = Chassis.From(configuration)
    .UsingNEventStore()
    .UsingInMemoryEventDispatcher()
    .UsingInMemoryEventStore() 
    .Build();
neChasis.Start();
```
