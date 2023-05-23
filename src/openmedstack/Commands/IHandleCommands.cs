// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHandleCommands.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the base interface for command handlers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines the base interface for command handlers.
/// </summary>
public interface IHandleCommands : IDisposable
{
}

/// <summary>
/// Defines the generic interface for command handlers.
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> of <see cref="DomainCommand"/> to handle.</typeparam>
public interface IHandleCommands<in T> : IHandleCommands
    where T : DomainCommand
{
    /// <summary>
    /// Handles the passed <see cref="DomainCommand"/>.
    /// </summary>
    /// <param name="command">The <see cref="DomainCommand"/> to handle.</param>
    /// <param name="headers">The messages headers associated with the command.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> of the handle operation.</param>
    /// <returns>A <see cref="Task"/> encapsulating the command response operation.</returns>
    Task<CommandResponse> Handle(T command, IMessageHeaders headers, CancellationToken cancellationToken = default);
}