// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandConsumer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the CommandConsumer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MassTransit;
using OpenMedStack.Commands;

namespace OpenMedStack.Autofac.MassTransit;

[ExcludeFromCodeCoverage]
internal class CommandConsumer<T> : IConsumer<T>
    where T : DomainCommand
{
    private readonly IHandleCommands<T> _commandHandler;

    public CommandConsumer(IHandleCommands<T> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public async Task Consume(ConsumeContext<T> context)
    {
        var response = await _commandHandler.Handle(
                context.Message,
                new MessageHeaders(context.Headers.GetAll()),
                context.CancellationToken)
            .ConfigureAwait(false);

        await context.RespondAsync(response).ConfigureAwait(false);
    }
}