namespace OpenMedStack.Autofac.MassTransit;

using System;
using System.Linq;
using System.Reflection;
using global::Autofac;
using global::Autofac.Core;
using global::MassTransit;

internal static class ConsumerExtensions
{
    public static void RegisterConsumers(this IReceiveEndpointConfigurator e, IComponentContext c)
    {
        var ctx = c.Resolve<ILifetimeScope>().BeginLifetimeScope();

        Type[] FindType(IComponentContext context, Func<Type, bool> filter) =>
            context.ComponentRegistry.Registrations
                .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => (r, s))
                .Where(rs => filter(rs.s.ServiceType))
                .Select(rs => rs.s.ServiceType)
                .ToArray();

        foreach (var consumerType in FindType(
            ctx,
            type => type.IsClass
             && type.GetTypeInfo()
                    .GetInterfaces()
                    .Any(
                        t => t.IsGenericType
                         && typeof(IConsumer<>).IsAssignableFrom(t.GetGenericTypeDefinition()))))
        {
            e.Consumer(consumerType, t => ctx.Resolve(t));
        }
    }
}