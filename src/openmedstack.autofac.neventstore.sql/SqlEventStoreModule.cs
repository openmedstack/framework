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
    using OpenMedStack.NEventStore;
    using OpenMedStack.NEventStore.Persistence.Sql;
    using OpenMedStack.NEventStore.Serialization;

    public class SqlEventStoreModule : Module
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _dbProviderFactory;
        private readonly ISqlDialect _dialect;
        private readonly bool _compress;

        public SqlEventStoreModule(string connectionString, DbProviderFactory dbProviderFactory, ISqlDialect dialect, bool compress = true)
        {
            _connectionString = connectionString;
            _dbProviderFactory = dbProviderFactory;
            _dialect = dialect;
            _compress = compress;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(
                    Wireup.Init()
                        .UsingSqlPersistence(_dbProviderFactory, _connectionString)
                        .WithDialect(_dialect)
                        .UsingJsonSerialization()
                        .LinkToAutofac(builder)
                        .Build())
                .As<IStoreEvents>()
                .SingleInstance();
        }
    }
}
