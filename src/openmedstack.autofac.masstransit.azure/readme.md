# OpenMedStack.Autofac.MassTransit.Azure

This package defines the core implementations of the MassTransit Azure integration for message publication and consumption.

## Usage

```csharp
var azureChasis = Chassis.From(configuration)
    .UsingMassTransitOverAzure()
    .Build();
azureChasis.Start();
```
