# OpenMedStack.Autofac.MassTransit.Sqs

This package defines the core implementations of the MassTransit AWS SQS integration for message publication and consumption.

## Usage

```csharp
var sqsChasis = Chassis.From(configuration)
    .UsingMassTransitOverSqs()
    .Build();
sqsChasis.Start();
```
