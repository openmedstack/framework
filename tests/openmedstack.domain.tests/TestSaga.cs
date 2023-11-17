// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSaga.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the Blob type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Domain.Tests;

using System;
using OpenMedStack.NEventStore.Abstractions;
using OpenMedStack.NEventStore.Abstractions.Persistence;

internal class TestSaga : SagaBase
{
    /// <inheritdoc />
    public TestSaga(string id, IEventStream stream)
        : base(id, stream)
    {
    }

    public bool EventHandled { get; private set; }

    public void Apply(TestEvent evt)
    {
        EventHandled = true;
        var cmd = new TestCommand("abc", 1, DateTimeOffset.UtcNow, Id);
        Dispatch(cmd);
    }
}
