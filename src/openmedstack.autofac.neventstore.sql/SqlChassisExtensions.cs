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
        public static Chassis UsingSqlEventStore(this Chassis chassis, DbProviderFactory dbProviderFactory, ISqlDialect dialect)
        {
            return chassis.AddAutofacModules(
                     (c, _) => new SqlEventStoreModule(c.ConnectionString, dbProviderFactory, dialect));
        }
    }
}
