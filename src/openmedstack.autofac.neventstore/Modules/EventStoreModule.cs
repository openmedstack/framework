﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventStoreModule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the module for configuring an NEventStore based event store in the project.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.Modules;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using global::Autofac;
using OpenMedStack.Autofac.NEventstore.Domain;
using OpenMedStack.Autofac.NEventstore.Repositories;
using OpenMedStack.Domain;
using OpenMedStack.NEventStore.Abstractions;

/// <summary>
/// Defines the module for configuring an NEventStore based event store in the project.
/// </summary>
public class EventStoreModule : global::Autofac.Module
{
    private readonly Assembly[] _assemblies;
    private readonly Func<IDetectConflicts> _conflictDetector;

    /// <summary>
    /// Initializes a new instance of an <see cref="EventStoreModule"/> class.
    /// </summary>
    /// <param name="conflictDetector">The <see cref="IDetectConflicts"/> to use to handle event conflicts.</param>
    /// <param name="assemblies">The list of <see cref="Assembly"/> to scan for aggregates and sagas.</param>
    public EventStoreModule(
        Func<IDetectConflicts>? conflictDetector = null,
        params Assembly[] assemblies)
    {
        _assemblies = assemblies.Concat(typeof(AggregateRootBase<>).Assembly)
            .DistinctBy((a, b) => string.Equals(a.FullName, b.FullName))
            .ToArray();
        _conflictDetector = () => conflictDetector?.Invoke() ?? new ConflictDetector();
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "Assembly scanning is required for DI.")]
    protected override void Load(ContainerBuilder builder)
    {
        Contract.Assume(builder != null);

        base.Load(builder);

        builder.RegisterAssemblyTypes(_assemblies).AssignableTo<IAggregate>().AsSelf();
        builder.RegisterAssemblyTypes(_assemblies).AssignableTo<ISaga>().AsSelf();
        builder.RegisterType<ContainerAggregateFactory>().As<IConstructAggregates>().SingleInstance();
        builder.RegisterInstance(_conflictDetector.Invoke()).As<IDetectConflicts>().SingleInstance();
        builder.RegisterType<SagaFactory>().As<IConstructSagas>().SingleInstance();
        builder.RegisterType<DefaultEventStoreRepository>().As<IRepository>().SingleInstance();
        builder.RegisterType<SagaEventStoreRepository>().As<ISagaRepository>().SingleInstance();
        builder.RegisterType<NesJsonSerializer>().As<ISerialize>().SingleInstance();
    }
}
