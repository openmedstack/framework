// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SagaFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the factory for constructing sagas.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Autofac;
using OpenMedStack.Domain;

namespace OpenMedStack.Autofac.NEventstore.Domain;

using OpenMedStack.NEventStore.Abstractions;

/// <summary>
/// Defines the factory for constructing sagas.
/// </summary>
internal class SagaFactory : IConstructSagas
{
    private readonly ILifetimeScope _lifetimeScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="SagaFactory"/> class.
    /// </summary>
    /// <param name="lifetimeScope">The container to resolve dependencies from.</param>
    public SagaFactory(ILifetimeScope lifetimeScope)
    {
        _lifetimeScope = lifetimeScope;
    }

    /// <inheritdoc />
    public TSaga Build<TSaga>(string id, IEventStream stream) where TSaga : ISaga =>
        (TSaga)_lifetimeScope.Resolve(
            typeof(TSaga),
            new NamedParameter("id", id),
            new NamedParameter("stream", stream));
}
