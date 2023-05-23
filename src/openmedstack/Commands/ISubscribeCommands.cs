// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISubscribeCommands.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the command subscription interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Commands;

using System;
using System.Threading.Tasks;

/// <summary>
/// Defines the command subscription interface.
/// </summary>
public interface ISubscribeCommands<out T> : IDisposable
    where T : DomainCommand
{
    /// <summary>
    /// Subscribes to command messages with the passed handler.
    /// </summary>
    /// <typeparam name="T">The command to subscribe to.</typeparam>
    /// <param name="handler">The handler to use for handling command messages.</param>
    /// <returns>An <see cref="IDisposable"/> subscription token.</returns>
    Task<IDisposable> Subscribe(IHandleCommands<T> handler);
}