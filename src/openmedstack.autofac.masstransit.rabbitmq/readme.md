# OpenMedStack.Autofac.MassTransit.RabbitMq

This package defines the core implementations of the MassTransit RabbitMQ integration for message publication and consumption.

## Usage

```csharp
var rabbitMqChasis = Chassis.From(configuration)
    .UsingMassTransitOverRabbitMq()
    .Build();
rabbitMqChasis.Start();
```
