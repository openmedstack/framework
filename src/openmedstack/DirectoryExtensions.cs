// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the directory extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

using System.IO;

/// <summary>
/// Defines the directory extensions.
/// </summary>
public static class DirectoryExtensions
{
    /// <summary>
    /// Gets whether the path is a directory.
    /// </summary>
    /// <param name="path">The path to examine.</param>
    /// <returns>True if the path is a directory path, otherwise false.</returns>
    public static bool IsDirectory(this string path) => Directory.Exists(path);

    /// <summary>
    /// Gets whether the path is a file.
    /// </summary>
    /// <param name="path">The path to examine.</param>
    /// <returns>True if the path is a file path, otherwise false.</returns>
    public static bool IsFile(this string path) => File.Exists(path);

    /// <summary>
    /// Create the directory if it does not exist.
    /// </summary>
    /// <param name="path">The requested directory path.</param>
    public static void EnsureDirectoryExists(this string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// Deletes the directory.
    /// </summary>
    /// <param name="path">The path to delete.</param>
    public static void DeleteDirectory(this string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    /// <summary>
    /// Gets whether the candidate directory path is a subdirectory of the other.
    /// </summary>
    /// <param name="candidate">The possible subdirectory.</param>
    /// <param name="other">The possible parent path.</param>
    /// <returns>True if the candidate is a subdirectory of the other, otherwise false.</returns>
    public static bool IsSubDirectoryOf(this string candidate, string other)
    {
        try
        {
            var candidateInfo = new DirectoryInfo(candidate);
            var otherInfo = new DirectoryInfo(other);

            while (candidateInfo.Parent != null)
            {
                if (candidateInfo.Parent.FullName == otherInfo.FullName.Trim(Path.DirectorySeparatorChar).Trim())
                {
                    return true;
                }

                candidateInfo = candidateInfo.Parent;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}
