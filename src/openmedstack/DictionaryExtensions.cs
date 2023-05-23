// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines extension methods for the <see cref="IDictionary{TKey,TValue}" /> type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

using System;
using System.Collections.Generic;

/// <summary>
/// Defines extension methods for the <see cref="IDictionary{TKey,TValue}"/> type.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Gets the requested value if it exists, otherwise the default value.
    /// </summary>
    /// <typeparam name="TKey">The <see cref="Type"/> of the dictionary key.</typeparam>
    /// <typeparam name="TValue">The <see cref="Type"/> of the dictionary value.</typeparam>
    /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}"/> to get the value from.</param>
    /// <param name="key">The key for the value.</param>
    /// <param name="defaultValue">The default value to use if key is not found.</param>
    /// <returns>The requested value, if found, or <c>null</c>.</returns>
    public static TValue GetOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue defaultValue) =>
        dictionary.TryGetValue(key, out var value) ? value : defaultValue;
}
