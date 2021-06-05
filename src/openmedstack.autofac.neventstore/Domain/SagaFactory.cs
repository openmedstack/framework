// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SagaFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the factory for constructing sagas.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Autofac;
using OpenMedStack.Domain;

namespace OpenMedStack.Autofac.NEventstore.Domain
{
    /// <summary>
    /// Defines the factory for constructing sagas.
    /// </summary>
    internal class SagaFactory : IConstructSagas
    {
        private readonly ILifetimeScope _lifetimeScope;

        public SagaFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public ISaga Build(Type type, string id) =>
            (ISaga)_lifetimeScope.Resolve(type, new NamedParameter("id", id));
    }
}
