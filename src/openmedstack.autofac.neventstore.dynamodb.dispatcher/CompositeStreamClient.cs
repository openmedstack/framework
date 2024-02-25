// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeStreamClient.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the $TYPE$ type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.DynamoDb.Dispatcher;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using OpenMedStack.Autofac.NEventstore.Domain;
using OpenMedStack.NEventStore.Abstractions;
using OpenMedStack.NEventStore.DynamoDbClient;

internal class CompositeStreamClient : StreamClient
{
    private readonly ICommitDispatcher[] _dispatchers;
    private readonly ILogger<CompositeStreamClient> _logger;

    public CompositeStreamClient(
        AWSCredentials credentials,
        AmazonDynamoDBStreamsConfig config,
        ISerialize serializer,
        IEnumerable<ICommitDispatcher> dispatchers,
        ILogger<CompositeStreamClient> logger)
        : base(credentials, config, serializer, logger)
    {
        _dispatchers = dispatchers.ToArray();
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task<HandlingResult> Handler(ICommit commit, CancellationToken cancellationToken)
    {
        var result = await Task.WhenAll(
            _dispatchers.Select(d => d.Dispatch(commit, cancellationToken)));
        return result.Max();
    }
}
