# OpenMedStack.Autofac.MassTransit.NEventStore.DynamoDB

This package defines the core implementations of DynamoDB storage for the OpenMedStack NEventStore.

This package is useful for production purposes.

## Usage

```csharp
var neChasis = Chassis.From(configuration)
    .UsingNEventStore()
    .UsingDynamoDbEventStore() 
    .Build();
neChasis.Start();
```
