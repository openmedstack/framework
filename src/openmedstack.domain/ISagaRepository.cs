namespace OpenMedStack.Domain;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines the saga repository.
/// </summary>
public interface ISagaRepository
{
    /// <summary>
    /// Gets the saga by id.
    /// </summary>
    /// <param name="sagaId">The id of the saga.</param>
    /// <param name="cancellationToken">The cancellation token for the async operation.</param>
    /// <typeparam name="TSaga">The <see cref="Type"/> of the saga.</typeparam>
    /// <returns>The constructed saga as an async operation.</returns>
    Task<TSaga> GetById<TSaga>(string sagaId, CancellationToken cancellationToken = default) where TSaga : ISaga;

    /// <summary>
    /// Saves the saga.
    /// </summary>
    /// <param name="saga">The saga to save.</param>
    /// <param name="updateHeaders">The aggregate headers.</param>
    /// <param name="cancellationToken">The cancellation token for the async operation.</param>
    /// <returns>The save operation as a <see cref="Task"/>.</returns>
    Task Save(ISaga saga, Action<IDictionary<string, object>>? updateHeaders = null, CancellationToken cancellationToken = default);
}
