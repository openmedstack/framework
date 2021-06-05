// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryStore.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the read store interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the read store interface.
    /// </summary>
    /// <typeparam name="TKey">The <see cref="Type"/> of key.</typeparam>
    /// <typeparam name="TItem">The <see cref="Type"/> of the item to read.</typeparam>
    public interface IQueryStore<in TKey, TItem> : IProvide<TKey, TItem>
    {
        /// <summary>
        /// Fetches the requested items.
        /// </summary>
        /// <param name="query">The filter query.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/> encapsulating the result sequence operation.</returns>
        IAsyncEnumerable<TItem> Fetch(Expression<Func<TItem, bool>> query, CancellationToken cancellationToken = default);
    }
}