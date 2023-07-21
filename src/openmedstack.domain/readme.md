# OpenMedStack.Domain

This package defines the core types for domain modeling. OpenMedStack supports modeling of systems as a system of aggregrates and sagas.

The recommendation is to use this package together with the `OpenMedStack NEventStore` implementation.

## Important Types

- `IAggregate`: Interface for aggregates. Aggregates are entities that are the root of a domain model. They are uniquely identified by an `Id` and can be modified by applying events.
- `AggregateRoot<TId>`: Base class for aggregate roots. Provides the `Id` property and the `Apply` method for applying events.
- `ISaga`: Interface for sagas. Sagas are entities that are not the root of a domain model. They are uniquely identified by an `Id` and can be modified by applying events.
- `Saga<TId>`: Base class for sagas. Provides the `Id` property and the `Apply` method for applying events.
