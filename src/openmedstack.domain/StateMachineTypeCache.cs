namespace OpenMedStack.Domain;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Stateless;

/// <summary>
/// Defines the type cache for state machines.
/// </summary>
/// <typeparam name="TState">The <see cref="Type"/> of state.</typeparam>
/// <typeparam name="TTrigger">The <see cref="Type"/> of trigger.</typeparam>
public class StateMachineTypeCache<TState, TTrigger>
{
    private readonly ConcurrentDictionary<Type, MethodInfo> _fireMethods = new();
    private readonly ConcurrentDictionary<Type, object> _parameters = new();

    /// <summary>
    /// Gets the parameter instance.
    /// </summary>
    /// <param name="messageType">The <see cref="Type"/> of message.</param>
    /// <param name="trigger">The trigger.</param>
    /// <returns></returns>
    public object GetParameterInstance(Type messageType, TTrigger trigger)
    {
        [UnconditionalSuppressMessage(
            "AOT",
            "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "<Pending>")]
        static object ValueFactory(Type t, TTrigger tr)
        {
            var typedInstance = typeof(StateMachine<,>.TriggerWithParameters<>).MakeGenericType(
                typeof(TState),
                typeof(TTrigger),
                t);
            return Activator.CreateInstance(typedInstance, tr)!;
        }

        return _parameters.GetOrAdd(messageType, ValueFactory, trigger);
    }

    /// <summary>
    /// Gets the state machine fire method to invoke.
    /// </summary>
    /// <param name="messageType">The message type.</param>
    /// <returns>The fire <see cref="MethodInfo"/>.</returns>
    [RequiresDynamicCode("Calls System.Reflection.MethodInfo.MakeGenericMethod(params Type[])")]
    public MethodInfo GetFireMethod(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
        Type messageType)
    {
        [RequiresDynamicCode("Calls System.Reflection.MethodInfo.MakeGenericMethod(params Type[])")]
        static MethodInfo ValueFactory(
            Type t,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] Type smt)
        {
            var genericFunc = smt.GetMethods()
                .First(
                    m =>
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
        }

        return _fireMethods.GetOrAdd(
            messageType,
            ValueFactory,
            typeof(StateMachine<TState, TTrigger>));
    }
}
