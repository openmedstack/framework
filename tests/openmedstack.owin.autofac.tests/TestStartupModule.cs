namespace OpenMedStack.Web.Autofac.Tests
{
    using global::Autofac;

    internal class TestStartupModule : Module
    {
        private readonly DeploymentConfiguration _deploymentConfiguration;

        public TestStartupModule(DeploymentConfiguration deploymentConfiguration)
        {
            _deploymentConfiguration = deploymentConfiguration;
        }

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_deploymentConfiguration).AsSelf().As<DeploymentConfiguration>().SingleInstance();
            builder.RegisterType<TestTenantProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TestCommandHandler>().AsImplementedInterfaces();
            builder.RegisterType<TestCommandConsumer>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<DummyTokenValidator>().AsImplementedInterfaces();
            builder.RegisterType<TestAggregate>().AsSelf();
        }
    }
}