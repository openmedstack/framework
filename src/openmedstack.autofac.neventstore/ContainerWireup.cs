namespace OpenMedStack.Autofac.NEventstore
{
    using System;
    using global::Autofac;
    using NEventStore;
    using NEventStore.Persistence;

    internal class ContainerWireup : Wireup
    {
        /// <inheritdoc />
        public ContainerWireup(Wireup inner, ContainerBuilder builder) : base(inner)
        {
            var persistStream = Container.Resolve<IPersistStreams>() ?? throw new Exception($"Could not resolve {nameof(IPersistStreams)}");
            builder.RegisterInstance(persistStream).As<IPersistStreams>().SingleInstance();
        }
    }
}