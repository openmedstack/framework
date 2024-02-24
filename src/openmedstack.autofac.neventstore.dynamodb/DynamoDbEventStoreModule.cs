// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamoDbEventStoreModule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the DynamoDbEventStoreModule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.DynamoDb;

using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using global::Autofac;
using OpenMedStack.NEventStore.Abstractions;
using OpenMedStack.NEventStore.DynamoDb;

internal sealed class DynamoDbEventStoreModule : Module
{
    private readonly AWSCredentials _awsCredentials;
    private readonly AmazonDynamoDBConfig _config;

    public DynamoDbEventStoreModule(AWSCredentials awsCredentials, AmazonDynamoDBConfig config)
    {
        _awsCredentials = awsCredentials;
        _config = config;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance<AmazonDynamoDBConfig>(_config).AsSelf().SingleInstance();
        builder.RegisterInstance(_awsCredentials).AsSelf().As<AWSCredentials>().SingleInstance();
        builder.RegisterType<AmazonDynamoDBClient>().AsSelf().As<IAmazonDynamoDB>().SingleInstance();
        builder.RegisterType<DynamoDbPersistenceEngine>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<DynamoDbManagement>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<DynamoDbSetup>().As<IBootstrapSystem>();
    }
}

internal class DynamoDbSetup : IBootstrapSystem
{
    private readonly IManagePersistence _managePersistence;

    public DynamoDbSetup(IManagePersistence managePersistence)
    {
        _managePersistence = managePersistence;
    }

    /// <inheritdoc />
    public uint Order { get; } = 1;

    /// <inheritdoc />
    public async Task Setup(CancellationToken cancellationToken)
    {
        await _managePersistence.Initialize();
    }

    /// <inheritdoc />
    public async Task Shutdown(CancellationToken cancellationToken)
    {
        await _managePersistence.Drop();
    }
}
