namespace OpenMedStack.Domain;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines the aggregate repository interface.
/// </summary>
public interface IRepository
{
    /// <summary>
    /// Gets the aggregate by id.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the async operation.</param>
    /// <typeparam name="TAggregate">The <see cref="Type"/> of the <see cref="IAggregate"/>.</typeparam>
    /// <returns>The constructed aggregate.</returns>
    Task<TAggregate> GetById<TAggregate>(string id, CancellationToken cancellationToken = default)
        where TAggregate : class, IAggregate;

    /// <summary>
    /// Gets the aggregate by id at a particular version.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="version">The aggregate version.</param>
    /// <param name="cancellationToken">The cancellation token for the async operation.</param>
    /// <typeparam name="TAggregate">The <see cref="Type"/> of the <see cref="IAggregate"/>.</typeparam>
    /// <returns>The constructed aggregate.</returns>
    Task<TAggregate> GetById<TAggregate>(string id, int version, CancellationToken cancellationToken = default)
        where TAggregate : class, IAggregate;

    /// <summary>
    /// Saves the aggregate.
    /// </summary>
    /// <param name="aggregate">The <see cref="IAggregate"/> to save.</param>
    /// <param name="updateHeaders">The aggregate headers.</param>
    /// <param name="cancellationToken">The cancellation token for the async operation.</param>
    /// <returns>The async operation as a <see cref="Task"/>.</returns>
    Task Save(
        IAggregate aggregate,
        Action<IDictionary<string, object>>? updateHeaders = null,
        CancellationToken cancellationToken = default);
}
