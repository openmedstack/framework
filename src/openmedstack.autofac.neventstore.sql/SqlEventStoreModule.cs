// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlEventStoreModule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the SqlEventStoreModule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.Sql;

using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using global::Autofac;
using Microsoft.Extensions.Logging;
using OpenMedStack.NEventStore.Abstractions;
using OpenMedStack.NEventStore.Persistence.Sql;

public sealed class SqlEventStoreModule<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TDialect> : Module
    where TDialect : ISqlDialect
{
    private readonly string _connectionString;
    private readonly DbProviderFactory _dbProviderFactory;

    public SqlEventStoreModule(string connectionString, DbProviderFactory dbProviderFactory)
    {
        _connectionString = connectionString;
        _dbProviderFactory = dbProviderFactory;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TDialect>().As<ISqlDialect>().SingleInstance();
        builder.Register(
                ctx => new SqlPersistenceEngine(
                    new NetStandardConnectionFactory(
                        _dbProviderFactory,
                        _connectionString,
                        ctx.Resolve<ILogger<NetStandardConnectionFactory>>()),
                    ctx.Resolve<ISqlDialect>(),
                    ctx.Resolve<ISerialize>(),
                    1000,
                    ctx.Resolve<IStreamIdHasher>(),
                    ctx.Resolve<ILogger<SqlPersistenceEngine>>()))
            .AsImplementedInterfaces()
            .SingleInstance();
    }
}
