﻿namespace OpenMedStack.Domain.Tests;

using System;
using OpenMedStack.Commands;

internal record TestCommand : DomainCommand
{
    /// <inheritdoc />
    public TestCommand(string aggregateId, int version, DateTimeOffset timeStamp, string? correlationId = null)
        : base(aggregateId, version, timeStamp, correlationId)
    {
    }
}

internal record TestCommand2 : DomainCommand
{
    /// <inheritdoc />
    public TestCommand2(string aggregateId, int version, DateTimeOffset timeStamp, string? correlationId = null)
        : base(aggregateId, version, timeStamp, correlationId)
    {
    }
}
