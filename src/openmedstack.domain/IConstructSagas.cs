// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITenantSagaRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the tenant based saga repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace OpenMedStack.Domain
{
    public interface IConstructSagas
    {
        ISaga Build(Type type, string id);
    }
}