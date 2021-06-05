// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinqExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the Linq extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Defines extensions methods for collections and sequences.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Filters the sequence for unique items given the passed predicate.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of item in the sequence.</typeparam>
        /// <param name="sequence">The sequence to filter.</param>
        /// <param name="predicate">The predicate to determine filtering.</param>
        /// <returns>A filtered sequence.</returns>
        public static IEnumerable<T> DistinctBy<T>(this IEnumerable<T> sequence, Func<T, T, bool> predicate) => sequence.Distinct(new FuncComparer<T>(predicate));

        /// <summary>Adds the passed items to the sequence.</summary>
        /// <param name="sequence">The sequence to extend.</param>
        /// <param name="items">The items to add to the sequence.</param>
        /// <typeparam name="T">The <see cref="Type"/> of items in the sequence.</typeparam>
        /// <returns>The extended <see cref="IEnumerable"/>.</returns>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> sequence, params T[] items) => sequence.Concat(items.AsEnumerable());

        /// <summary>Removes <c>null</c> items in the sequence.</summary>
        /// <param name="sequence">The sequence to clean.</param>
        /// <typeparam name="T">The <see cref="Type"/> of items in the sequence.</typeparam>
        /// <returns>The cleaned <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> NonNulls<T>(this IEnumerable<T?> sequence)
            where T : class
        {
            return sequence.Where(x => x != null).Select(x => x!);
        }

        public static void AddRange<T>(
          this ICollection<T> collection,
          IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static void AddRange<T>(
          this ICollection<T> collection,
          params T[] items)
        {
            AddRange(collection, items.AsEnumerable());
        }

        public static void RemoveAll<T>(
          this ICollection<T> collection,
          Func<T, bool> predicate)
        {
            var toRemove = collection.Where(predicate).ToArray();
            foreach (var item in toRemove)
            {
                collection.Remove(item);
            }
        }

        public static IEnumerable<T> ExceptWhere<T>(
          this IEnumerable<T> sequence,
          Func<T, bool> predicate) =>
          sequence.Where(x => !predicate(x));

        private class FuncComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _predicate;

            public FuncComparer(Func<T, T, bool> predicate)
            {
                _predicate = predicate;
            }

            public bool Equals([AllowNull] T x, [AllowNull] T y)
            {
                if (x == null)
                {
                    return y == null;
                }

                return y is object && _predicate.Invoke(x, y);
            }

            public int GetHashCode(T obj) => obj!.GetHashCode();
        }
    }
}