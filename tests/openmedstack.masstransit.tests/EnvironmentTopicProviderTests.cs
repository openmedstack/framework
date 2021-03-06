namespace openmedstack.masstransit.tests;

using OpenMedStack;
using OpenMedStack.Autofac.MassTransit;
using Xunit;

public class EnvironmentTopicProviderTests
{
    [Fact]
    public void CanGetTopic()
    {
        var tenantProvider = new ConfigurationTenantProvider(new DeploymentConfiguration { TenantPrefix = "Test" });
        var provider = new EnvironmentTopicProvider(tenantProvider);

        var topic = provider.GetTenantSpecific<TestEvent>();

        Assert.Equal("TestTest", topic);
    }

    [Fact]
    public void CanGetCanonicalTopic()
    {
        var tenantProvider = new ConfigurationTenantProvider(new DeploymentConfiguration { TenantPrefix = "Test" });
        var provider = new EnvironmentTopicProvider(tenantProvider);

        var topic = provider.Get<TestEvent>();

        Assert.Equal("Test", topic);
    }
}
