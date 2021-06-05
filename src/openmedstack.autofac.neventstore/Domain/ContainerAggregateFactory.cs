// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerAggregateFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the ContainerAggregateFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Autofac;
using OpenMedStack.Autofac.NEventstore.Repositories;
using OpenMedStack.Domain;

namespace OpenMedStack.Autofac.NEventstore.Domain
{
    internal class ContainerAggregateFactory : IConstructAggregates
    {
        private readonly ILifetimeScope _container;

        public ContainerAggregateFactory(ILifetimeScope container)
        {
            _container = container;
        }

        public IAggregate Build(Type type, string id, IMemento? snapshot)
        {
            var instance = (IAggregate)_container.Resolve(
                type,
                new TypedParameter(typeof(string), id),
                new TypedParameter(typeof(IMemento), snapshot));

            return instance;
        }
    }
}