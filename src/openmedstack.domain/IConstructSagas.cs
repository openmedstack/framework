// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITenantSagaRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the tenant based saga repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace OpenMedStack.Domain;

/// <summary>
/// Defines the saga factory
/// </summary>
public interface IConstructSagas
{
    /// <summary>
    /// Builds a new saga of the specified type and id.
    /// </summary>
    /// <param name="id">The saga identifier.</param>
    /// <returns>The constructed saga.</returns>
    TSaga Build<TSaga>(string id) where TSaga : ISaga;
}
