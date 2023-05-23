// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the HashExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

using System;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Defines extensions methods for hashing.
/// </summary>
public static class HashExtensions
{
    /// <summary>Creates a SHA256 hash of the specified input.</summary>
    /// <param name="input">The input.</param>
    /// <param name="salt">The salt value for the hash calculation.</param>
    /// <returns>A hash value.</returns>
    public static string Sha256(this string input, string salt = "")
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input + salt);
        return BitConverter.ToString(sha256.ComputeHash(bytes)).Replace("-", string.Empty);
    }

    /// <summary>Creates a SHA256 hash of the specified input.</summary>
    /// <param name="buffer">The input.</param>
    /// <returns>A hash value.</returns>
    public static string Sha256(this byte[] buffer)
    {
        if (buffer == null)
        {
            return string.Empty;
        }

        using var sha256 = SHA256.Create();
        return BitConverter.ToString(sha256.ComputeHash(buffer)).Replace("-", string.Empty);
    }

    /// <summary>Creates a SHA512 hash of the specified input.</summary>
    /// <param name="input">The input.</param>
    /// <param name="salt">The salt value for the hash calculation.</param>
    /// <returns>A hash value.</returns>
    public static string Sha512(this string input, string salt = "")
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        using var sha512 = SHA512.Create();
        var bytes = Encoding.UTF8.GetBytes(input + salt);
        return BitConverter.ToString(sha512.ComputeHash(bytes)).Replace("-", string.Empty);
    }
}