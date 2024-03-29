﻿namespace OpenMedStack.Tests.Startup;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using OpenMedStack.Autofac.Startup;
using OpenMedStack.Events;
using Xunit;

public class EventHandlerCreationValidatorTests
{
    [Fact]
    public async Task WhenEventHandlerCreationThrowsThenReturnsError()
    {
        var createFunc = new Func<IEnumerable<IHandleEvents>>(() => throw new Exception("test error"));

        var validator = new EventHandlerCreationValidator(
            createFunc,
            NullLogger<EventHandlerCreationValidator>.Instance);
        var result = await validator.Validate();

        Assert.NotNull(result);
    }

    [Fact]
    public async Task WhenEventHandlerCreationSucceedsThenDoesNotReturnError()
    {
        var createFunc = new Func<IEnumerable<IHandleEvents>>(() => new[] { new TestEventHandler() });

        var validator = new EventHandlerCreationValidator(
            createFunc,
            NullLogger<EventHandlerCreationValidator>.Instance);
        var result = await validator.Validate();

        Assert.Null(result);
    }
}
