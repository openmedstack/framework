// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandHandlerBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the abstract base class for handling domain events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using OpenMedStack.Commands;
using Microsoft.Extensions.Logging;

/// <summary>
/// Defines the abstract base class for handling domain events.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CommandHandlerBase<T> : IHandleCommands<T>
    where T : DomainCommand
{
    private readonly IRepository _repository;
    private readonly IValidateTokens _tokenValidator;
    private readonly ILogger<CommandHandlerBase<T>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandHandlerBase{T}"/> class.
    /// </summary>
    /// <param name="repository">The <see cref="IRepository"/> to load aggregates from.</param>
    /// <param name="tokenValidator">The <see cref="IValidateTokens"/> used to read message tokens.</param>
    /// <param name="logger">The logger</param>
    protected CommandHandlerBase(
        IRepository repository,
        IValidateTokens tokenValidator,
        ILogger<CommandHandlerBase<T>> logger)
    {
        _repository = repository;
        _tokenValidator = tokenValidator;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(
        T command,
        IMessageHeaders headers,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Handling {name} command. Correlation ID: {correlationId}",
            typeof(T).Name,
            command.CorrelationId);

        cancellationToken.ThrowIfCancellationRequested();

        var userToken = _tokenValidator.Validate(headers.UserToken);
        if (!await VerifyUserToken(userToken).ConfigureAwait(false))
        {
            _logger.LogError("Invalid authentication token received with command: {userToken}", userToken);
            return new CommandResponse(
                command.AggregateId,
                command.Version,
                "Not authorized",
                command.CorrelationId);
        }

        try
        {
            var response = await HandleInternal(command, headers, cancellationToken).ConfigureAwait(false);

            return response;
        }
        catch (Exception exception)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogError(exception, exception.Message);

            return await OnException(exception, command, headers, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Verifies that the user token passed with the command is valid and authorizes the action.
    /// </summary>
    /// <param name="token">The user token. The token may be <c>null</c> if no token is sent with the message.</param>
    /// <returns><c>true</c> is the authentication is successful, otherwise <c>false</c>.</returns>
    protected virtual Task<bool> VerifyUserToken(IdentityToken? token) => Task.FromResult(true);

    /// <summary>
    /// The handling method to be overridden in inherited classes.
    /// </summary>
    /// <param name="command">The <see cref="DomainCommand"/> to handle.</param>
    /// <param name="headers">The messages headers associated with the command.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the async operation.</param>
    /// <returns></returns>
    protected abstract Task<CommandResponse> HandleInternal(
        T command,
        IMessageHeaders headers,
        CancellationToken cancellationToken);

    protected virtual Task<CommandResponse> OnException(
        Exception exception,
        T command,
        IMessageHeaders headers,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(command.CreateResponse(exception.Message));

    /// <summary>
    /// Gets the <see cref="AggregateRootBase{T}"/> with the requested id.
    /// </summary>
    /// <typeparam name="TAggregate">The requested aggregate as the latest persisted version.</typeparam>
    /// <param name="id">The id of the aggregate.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the async operation.</param>
    /// <returns>The instance of the aggregate, or null if the aggregate does not exist.</returns>
    protected Task<TAggregate> Get<TAggregate>(string id, CancellationToken cancellationToken = default)
        where TAggregate : class, IAggregate =>
        _repository.GetById<TAggregate>(id, cancellationToken);

    /// <summary>
    /// Saves the passed aggregate in the repository.
    /// </summary>
    /// <typeparam name="TAggregate">The <see cref="Type"/> of the aggregate to save.</typeparam>
    /// <param name="aggregate">The <see cref="AggregateRootBase{T}"/> to save.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the async operation.</param>
    /// <returns></returns>
    protected Task Save<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken)
        where TAggregate : class, IAggregate =>
        _repository.Save(aggregate, cancellationToken: cancellationToken);

    /// <summary>
    /// Disposes the command handler.
    /// </summary>
    /// <param name="isDisposing"></param>
    protected virtual void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
        }
    }
}
