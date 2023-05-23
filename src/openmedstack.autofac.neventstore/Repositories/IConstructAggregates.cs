namespace OpenMedStack.Autofac.NEventstore.Repositories;

using System;
using OpenMedStack.Domain;

public interface IConstructAggregates
{
    IAggregate Build(Type type, string id, IMemento? snapshot);
}
