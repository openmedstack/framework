namespace OpenMedStack.Framework.IntegrationTests
{
    using global::Autofac;
    using OpenMedStack.Autofac;

    internal class TestModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<TestTenantProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DummyTokenValidator>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<EventSubscriberValidator>().AsImplementedInterfaces();
        }
    }
}