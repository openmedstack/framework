// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlChassisExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the SqlChassisExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.Sql
{
    using System.Data.Common;
    using NEventStore.Persistence.Sql;

    public static class SqlChassisExtensions
    {
        public static Chassis<TConfiguration> UsingSqlEventStore<TConfiguration>(
            this Chassis<TConfiguration> chassis,
            DbProviderFactory dbProviderFactory,
            ISqlDialect dialect)
            where TConfiguration : DeploymentConfiguration
        {
            return chassis.AddAutofacModules(
                (c, _) => new SqlEventStoreModule(c.ConnectionString, dbProviderFactory, dialect));
        }
    }
}
