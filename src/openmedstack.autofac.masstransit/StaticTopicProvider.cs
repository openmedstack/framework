// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvironmentTopicProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the EnvironmentTopicProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

internal class StaticTopicProvider : IProvideTopic
{
    private readonly IProvideTenant _tenantProvider;
    private readonly IDictionary<string, string> _topicMap;

    public StaticTopicProvider(IProvideTenant tenantProvider, IDictionary<string, string>? topicMap = null)
    {
        _tenantProvider = tenantProvider;
        _topicMap = topicMap ?? new Dictionary<string, string>();
    }

    /// <inheritdoc />
    public string GetTenantSpecific<T>()
    {
        var tenant = _tenantProvider.GetTenantName();
        return tenant + Get<T>();
    }

    /// <inheritdoc />
    public string Get<T>()
    {
        var type = typeof(T);
        var fullName = PrettyName(type);
        if (_topicMap.TryGetValue(fullName, out var topic))
        {
            return topic;
        }

        var topicAttribute = type.GetCustomAttribute<TopicAttribute>();
        var result = topicAttribute?.Topic ?? fullName;
        return _topicMap.TryAdd(fullName, result) ? result : _topicMap[fullName];
    }

    private static string PrettyName(Type type)
    {
        if (type.GetGenericArguments().Length == 0)
        {
            return type.FullName ?? type.Name;
        }

        var genericArguments = type.GetGenericArguments();
        var typeDefinition = type.FullName ?? type.Name;
        var unmangledName = typeDefinition[..typeDefinition.IndexOf("`", StringComparison.Ordinal)];
        return $"{unmangledName}<{string.Join(",", genericArguments.Select(PrettyName))}>";
    }
}
