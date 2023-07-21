# OpenMedStack.MassTransit

This package defines the core implementations of the MassTransit integration for message publication and consumption.
The transport is based on the [MassTransit](https://masstransit-project.com/) library, and extends it to exchange messages formatted as CloudEvents internally.

## Usage

```csharp
var massTransitChasis = Chassis.From(configuration)
    .UsingInMemoryMassTransit()
    .Build();
massTransitChasis.Start();
```
