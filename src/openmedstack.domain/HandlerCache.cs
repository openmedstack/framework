// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HandlerCache.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the Blob type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Domain;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

internal class HandlerCache
{
    private readonly ConcurrentDictionary<Type, (Type, MethodInfo)[]> _handlers =
        new();

    private HandlerCache()
    {
    }

    public static HandlerCache Instance { get; } = new HandlerCache();

    public (Type messageType, MethodInfo method)[] GetHandlers(Type type)
    {
        (Type, MethodInfo)[] ValueFactory(Type t) =>
            (from m in t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
             where m.Name == "Apply"
              && m.GetParameters().Length == 1
              && m.ReturnParameter?.ParameterType == typeof(void)
             let messageType = m.GetParameters().Single().ParameterType
             select (messageType, m)).ToArray();

        return _handlers.GetOrAdd(type, ValueFactory);
    }
}
