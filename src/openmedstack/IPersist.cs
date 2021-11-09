// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersist.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the persistance interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the persistence interface.
    /// </summary>
    /// <typeparam name="TKey">The <see cref="Type"/> of key.</typeparam>
    /// <typeparam name="TItem">The <see cref="Type"/> of item to persist.</typeparam>
    public interface IPersist<TKey, in TItem> : IDisposable
        where TItem : class
    {
        /// <summary>
        /// Persists the given item.
        /// </summary>
        /// <param name="item">The item to persist.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the save <see cref="Task"/>.</param>
        /// <returns>The <see cref="Task"/> encapsulating the save operation.</returns>
        Task<TKey> Save(TItem item, CancellationToken cancellationToken = default);
    }
}