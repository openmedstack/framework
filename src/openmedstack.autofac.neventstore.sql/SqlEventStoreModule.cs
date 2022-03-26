// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlEventStoreModule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the SqlEventStoreModule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.Sql
{
    using System.Data.Common;
    using global::Autofac;
    using global::NEventStore;
    using Microsoft.Extensions.Logging;
    using OpenMedStack.NEventStore;
    using OpenMedStack.NEventStore.Persistence;
    using OpenMedStack.NEventStore.Persistence.Sql;
    using OpenMedStack.NEventStore.Serialization;

    public class SqlEventStoreModule : Module
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _dbProviderFactory;
        private readonly ISqlDialect _dialect;

        public SqlEventStoreModule(string connectionString, DbProviderFactory dbProviderFactory, ISqlDialect dialect)
        {
            _connectionString = connectionString;
            _dbProviderFactory = dbProviderFactory;
            _dialect = dialect;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(
                    ctx => Wireup.Init(ctx.Resolve<ILogger<Wireup>>())
                        .UsingSqlPersistence(_dbProviderFactory, _connectionString)
                        .WithDialect(_dialect)
                        .UsingJsonSerialization()
                        .LinkToAutofac(builder)
                        .Build())
                .As<IStoreEvents>()
                .SingleInstance();
            builder.Register(ctx => ctx.Resolve<IStoreEvents>().Advanced).As<IPersistStreams>();
        }
    }
}
