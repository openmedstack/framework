// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSaga.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the Blob type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace OpenMedStack.Domain.Tests
{
    using System;
    using OpenMedStack.Commands;
    using OpenMedStack.Events;

    internal class TestSaga : SagaBase<DomainEvent, DomainCommand>
    {
        /// <inheritdoc />
        public TestSaga(string id)
            : base(id)
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
}
