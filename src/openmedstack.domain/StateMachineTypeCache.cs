namespace OpenMedStack.Domain;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Stateless;

public class StateMachineTypeCache<TState, TTrigger>
{
    private readonly ConcurrentDictionary<Type, MethodInfo> _fireMethods = new();
    private readonly ConcurrentDictionary<Type, object> _parameters = new();

    public object GetParameterInstance(Type messageType, TTrigger trigger)
    {
        return _parameters.GetOrAdd(messageType, static (t, tr) =>
        {
            var typedInstance =
                typeof(StateMachine<,>.TriggerWithParameters<>).MakeGenericType(typeof(TState), typeof(TTrigger),
                    t);
            return Activator.CreateInstance(typedInstance, tr)!;
        }, trigger);
    }

    public MethodInfo GetFireMethod(Type messageType)
    {
        return _fireMethods.GetOrAdd(messageType, static (t, smt) =>
        {
            var genericFunc = smt.GetMethods().First(m =>
            {
                if (!m.IsGenericMethod || m.Name != "Fire")
                {
                    return false;
                }

                var parameterInfos = m.GetParameters();
                return parameterInfos is
                    [{ ParameterType.IsGenericType: true }, { ParameterType.IsGenericType: false }];
            });

            return genericFunc.MakeGenericMethod(t);
        }, typeof(StateMachine<TState, TTrigger>));
    }
}