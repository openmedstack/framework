// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProvide.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the provider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines the provider interface.
/// </summary>
/// <typeparam name="TItem">The <see cref="Type"/> of item to fetch.</typeparam>
public interface IProvide<TItem> : IDisposable
{
    /// <summary>
    /// Fetches the item.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the asynchronous operation.</param>
    /// <returns>A <see cref="Task"/> encapsulating the fetch operation.</returns>
    Task<TItem> Fetch(CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines the provider interface.
/// </summary>
/// <typeparam name="TKey">The <see cref="Type"/> of key.</typeparam>
/// <typeparam name="TItem">The <see cref="Type"/> of item to fetch.</typeparam>
public interface IProvide<in TKey, TItem> : IDisposable
{
    /// <summary>
    /// Fetches the item with the given key.
    /// </summary>
    /// <param name="key">The key of the item to fetch.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the asynchronous operation.</param>
    /// <returns>A <see cref="Task"/> encapsulating the fetch operation.</returns>
    Task<TItem?> Fetch(TKey key, CancellationToken cancellationToken = default);
}
