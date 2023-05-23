// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProvideTopic.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the IProvideTopics interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

using System;

/// <summary>
/// Defines the topic provider interface
/// </summary>
public interface IProvideTopic
{
    /// <summary>
    /// Gets the topic for the provided <see cref="Message"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the message</typeparam>
    /// <returns>The topic as a <see cref="string"/>.</returns>
    string GetTenantSpecific<T>();

    /// <summary>
    /// Gets the non-tenant specific topic for the provided <see cref="Message"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the message</typeparam>
    /// <returns>The topic as a <see cref="string"/>.</returns>
    string Get<T>();
}