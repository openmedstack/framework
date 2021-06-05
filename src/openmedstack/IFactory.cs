// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the factory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack
{
    using System;

    /// <summary>
    /// Defines the factory interface.
    /// </summary>
    /// <typeparam name="TResult">The <see cref="Type"/> of the instance to create.</typeparam>
    public interface IFactory<out TResult> : IDisposable
    {
        /// <summary>
        /// The create mthod.
        /// </summary>
        /// <returns>An instance of <see cref="Type"/>. The instance will not be null.</returns>
        TResult Create();
    }

    /// <summary>
    /// Defines the parameterized factory interface.
    /// </summary>
    /// <typeparam name="TParameter">The <see cref="Type"/> of the creation parameter.</typeparam>
    /// <typeparam name="TResult">The <see cref="Type"/> of the instance to create.</typeparam>
    public interface IFactory<in TParameter, out TResult> : IDisposable
    {
        /// <summary>
        /// The create mthod.
        /// </summary>
        /// <param name="parameter">The creation parameter.</param>
        /// <returns>An instance of <see cref="Type"/>. The instance will not be null.</returns>
        TResult Create(TParameter parameter);
    }
}