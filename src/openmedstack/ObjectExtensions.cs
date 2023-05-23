// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the ObjectExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

using System;

/// <summary>
/// Defines extension methods for objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Disposes an object if it is an <see cref="IDisposable"/>.
    /// </summary>
    /// <param name="obj">The object to try to dispose.</param>
    public static void TryDispose(this object? obj)
    {
        if (obj is IDisposable d)
        {
            d.Dispose();
        }
    }
}