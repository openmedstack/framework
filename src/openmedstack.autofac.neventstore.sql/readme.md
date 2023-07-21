# OpenMedStack.Autofac.MassTransit.NEventStore.Sql

This package defines the core implementations of SQL based RDBMS storage for the OpenMedStack NEventStore.

This package is useful for production purposes.

## Usage

```csharp
var neChasis = Chassis.From(configuration)
    .UsingNEventStore()
    .UsingInMemoryEventDispatcher()
    .UsingSqlEventStore() 
    .Build();
neChasis.Start();
```
