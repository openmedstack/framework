namespace OpenMedStack.Domain.Tests;

using global::Autofac;

internal class TestModule : Module
{
    private readonly DeploymentConfiguration _configuration;

    public TestModule(DeploymentConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(_configuration).SingleInstance();
        builder.RegisterType<TestDataStore>().AsSelf().SingleInstance();
        builder.RegisterType<ConfigurationTenantProvider>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<DummyTokenValidator>().AsImplementedInterfaces();
    }
}
