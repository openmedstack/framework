# OpenMedStack.Autofac.MassTransit.ActiveMq

This package defines the core implementations of the MassTransit ActiveMQ integration for message publication and consumption.

## Usage

```csharp
var activeMqChasis = Chassis.From(configuration)
    .UsingMassTransitOverActiveMq()
    .Build();
activeMqChasis.Start();
```
