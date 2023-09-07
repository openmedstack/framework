# OpenMedStack.Autofac.MassTransit.Grpc

This package defines the core implementations of the MassTransit RabbitMQ integration for message publication and consumption.

## Usage

```csharp
var grpcMqChasis = Chassis.From(configuration)
    .UsingMassTransitOverGrpc()
    .Build();
grpcMqChasis.Start();
```
