namespace OpenMedStack.Autofac.NEventstore.DynamoDb;

using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using OpenMedStack.Autofac.NEventstore.Domain;
using OpenMedStack.Autofac.NEventstore.DynamoDb.Dispatcher;
using OpenMedStack.NEventStore.Abstractions;
using OpenMedStack.NEventStore.DynamoDbClient;

internal class CompositeDynamoDbDispatcherSetup : IBootstrapSystem
{
    private readonly StreamClient _client;

    public CompositeDynamoDbDispatcherSetup(StreamClient client)
    {
        _client = client;
    }

    /// <inheritdoc />
    public uint Order { get; } = 100;

    /// <inheritdoc />
    public async Task Setup(CancellationToken cancellationToken)
    {
        await _client.Subscribe(cancellationToken);
    }

    /// <inheritdoc />
    public Task Shutdown(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
