// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilePathExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the FilePathExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack
{
    using System;
    using System.IO;

    /// <summary>
    /// Defines extensions methods for file paths.
    /// </summary>
    public static class FilePathExtensions
    {
        private static readonly string RootPath;

        static FilePathExtensions()
        {
            RootPath = !string.IsNullOrWhiteSpace(AppContext.BaseDirectory)
                ? AppContext.BaseDirectory
                : "./".ToFullOsPath();
        }

        /// <summary>
        /// Converts the given path to an OS compatible path.
        /// </summary>
        /// <param name="path">The path to convert.</param>
        /// <returns>The path in OS format.</returns>
        public static string ToFullOsPath(this string path)
        {
            var adapted = path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            return Path.GetFullPath(adapted);
        }

        public static string ToApplicationPath(this string? path) =>
            path == null ? string.Empty : Path.Combine(RootPath, path).ToFullOsPath();
    }
}
