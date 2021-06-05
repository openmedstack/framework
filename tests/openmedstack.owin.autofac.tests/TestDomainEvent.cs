﻿namespace OpenMedStack.Web.Autofac.Tests
{
    using System;
    using OpenMedStack.Events;

    public class TestDomainEvent : DomainEvent
    {
        /// <inheritdoc />
        public TestDomainEvent(string id, int version) : base(id, version, DateTimeOffset.Now)
        {
        }
    }
}