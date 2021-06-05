namespace OpenMedStack.Autofac.NEventstore
{
    using global::Autofac;
    using NEventStore;

    public static class WireupExtensions
    {
        public static Wireup LinkToAutofac(this Wireup wireup, ContainerBuilder builder) => new ContainerWireup(wireup, builder);
    }
}