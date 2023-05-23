// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRouteCommands.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the interface a command router.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Commands;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines the interface a command router.
/// </summary>
public interface IRouteCommands
{
    /// <summary>
    /// Sends the <see cref="DomainCommand"/> to the intended destination.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <see cref="DomainCommand"/> to send.</typeparam>
    /// <param name="command">The <see cref="DomainCommand"/> instance.</param>
    /// <param name="headers">The headers to accompany the command.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the task.</param>
    /// <returns>T <see cref="CommandResponse"/> as an asynchronous operation.</returns>
    Task<CommandResponse> Send<T>(T command, IDictionary<string, object>? headers = null, CancellationToken cancellationToken = default)
        where T : DomainCommand;
}