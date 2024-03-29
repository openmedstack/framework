﻿namespace OpenMedStack.Domain.Tests;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack.Commands;

internal class TestCommandHandler : CommandHandlerBase<TestCommand>
{
    private readonly TestDataStore _dataStore;

    /// <inheritdoc />
    public TestCommandHandler(IRepository repository, TestDataStore dataStore, IValidateTokens tokenValidator, ILogger<TestCommandHandler> logger)
        : base(repository, tokenValidator, logger)
    {
        _dataStore = dataStore;
    }

    /// <inheritdoc />
    protected override Task<CommandResponse> HandleInternal(TestCommand command, IMessageHeaders headers, CancellationToken cancellationToken)
    {
        _dataStore.Commands++;
        return Task.FromResult(command.CreateResponse());
    }
}

internal class TestCommand2Handler : CommandHandlerBase<TestCommand2>
{
    private readonly TestDataStore _dataStore;

    /// <inheritdoc />
    public TestCommand2Handler(IRepository repository, TestDataStore dataStore, IValidateTokens tokenValidator, ILogger<TestCommand2Handler> logger)
        : base(repository, tokenValidator, logger)
    {
        _dataStore = dataStore;
    }

    /// <inheritdoc />
    protected override Task<CommandResponse> HandleInternal(TestCommand2 command, IMessageHeaders headers, CancellationToken cancellationToken)
    {
        _dataStore.Commands++;
        return Task.FromResult(command.CreateResponse());
    }
}
