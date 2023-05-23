// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBootstrapSystem.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the bootstrap interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines the bootstrap interface.
/// </summary>
public interface IBootstrapSystem
{
    /// <summary>
    /// Gets the order of bootstrapping. Higher is later.
    /// </summary>
    uint Order { get; }

    /// <summary>
    /// The <see cref="Task"/> to perform on setup.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> encapsulating the setup actions.</returns>
    Task Setup(CancellationToken cancellationToken);

    /// <summary>
    /// The <see cref="Task"/> to perform on shutdown.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> encapsulating the shutdown actions.</returns>
    Task Shutdown(CancellationToken cancellationToken);
}