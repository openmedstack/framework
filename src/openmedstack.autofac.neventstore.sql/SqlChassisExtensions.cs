// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlChassisExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the SqlChassisExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.Sql;

using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using NEventStore.Persistence.Sql;

public static class SqlChassisExtensions
{
    public static Chassis<TConfiguration> UsingSqlEventStore<TConfiguration,
                                                             [DynamicallyAccessedMembers(
                                                                 DynamicallyAccessedMemberTypes.PublicConstructors)]
                                                             TDialect>(
        this Chassis<TConfiguration> chassis,
        DbProviderFactory dbProviderFactory)
        where TConfiguration : DeploymentConfiguration
        where TDialect : ISqlDialect
    {
        return chassis.AddAutofacModules(
            (c, _) => new SqlEventStoreModule<TDialect>(c.ConnectionString, dbProviderFactory));
    }
}
