namespace OpenMedStack.Autofac.NEventstore.Domain;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack.Events;
using OpenMedStack.NEventStore.Abstractions;
using OpenMedStack.ReadModels;

public sealed class ReadModelUpdater : IReadModelUpdater
{
    private readonly ILogger<ReadModelUpdater> _logger;
    private readonly ConcurrentDictionary<Type, MethodInfo> _updateMethods = new();
    private readonly ConcurrentDictionary<Type, IEnumerable<object>> _readModelUpdaters;

    public ReadModelUpdater(IEnumerable<IUpdateReadModel> readModelUpdaters, ILogger<ReadModelUpdater> logger)
    {
        _logger = logger;
        var updaters = readModelUpdaters.ToArray();

        _logger.LogDebug("Read model updater created with {Count} updaters", updaters.Length);

        var pairs = GroupUpdaters(updaters);
        _readModelUpdaters = new ConcurrentDictionary<Type, IEnumerable<object>>(pairs);

        _logger.LogDebug(
            "Read model updaters defined for {Names}",
            string.Join(Environment.NewLine, _readModelUpdaters.Keys.Select(x => x.Name)));
    }

    public async Task<bool> Update(ICommit commit)
    {
        try
        {
            using (_logger.BeginScope(commit))
            {
                var updateReadModelTasks = (from evt in commit.Events
                                            where evt.Body is BaseEvent
                                            let bodyType = GetBodyType(evt.Body)
                                            let headers =
                                                new MessageHeaders(
                                                    evt.Headers.Concat(
                                                        new KeyValuePair<string, object>(
                                                            Constants.CommitSequence,
                                                            commit.CheckpointToken)))
                                            let method =
                                                _updateMethods.GetOrAdd(bodyType, ValueFactory)
                                            from updater in _readModelUpdaters.GetOrAdd(
                                                bodyType,
                                                _ => Array.Empty<IUpdateReadModel>())
                                            let result = method.Invoke(
                                                updater,
                                                new[] { evt.Body, headers, CancellationToken.None })
                                            select (Task)result).ToArray();

                _logger.LogDebug(
                    "Invoking {Amount} read model updaters for commit {CommitId}",
                    updateReadModelTasks.Length,
                    commit.CommitId);

                await Task.WhenAll(updateReadModelTasks).ConfigureAwait(false);
                return true;
            }
        }
        catch (Exception exception)
        {
            _logger.LogError("{Error}", exception.Message);
            return false;
        }
    }

    [UnconditionalSuppressMessage(
        "AOT",
        "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
        Justification = "Generic method available")]
    private MethodInfo ValueFactory(Type t) =>
        typeof(IUpdateReadModel<>).MakeGenericType(t)
            .GetMethod("Update")!;

    private Type GetBodyType(object body)
    {
        var type = body.GetType();

        _logger.LogDebug("Event of type {Type} raised", type);

        return type;
    }

    private IEnumerable<KeyValuePair<Type, IEnumerable<object>>> GroupUpdaters(IEnumerable<IUpdateReadModel> updaters)
    {
        var groups = (from updater in updaters
                      from updaterInterface in updater.GetType().GetInterfaces()
                      where updaterInterface.IsGenericType
                      where typeof(IUpdateReadModel<>).IsAssignableFrom(updaterInterface.GetGenericTypeDefinition())
                      let eventType = updaterInterface.GetGenericArguments()[0]
                      group updater by eventType
                      into eventGroups
                      select new KeyValuePair<Type, IEnumerable<object>>(eventGroups.Key, eventGroups.ToArray()))
            .ToArray();

        foreach (var pair in groups)
        {
            _logger.LogDebug(
                "{Key} updates: {Types}",
                pair.Key.Name,
                string.Join(Environment.NewLine, pair.Value.Select(x => x.GetType().Name)));
        }

        return groups;
    }
}
