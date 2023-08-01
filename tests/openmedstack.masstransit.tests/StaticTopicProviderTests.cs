namespace openmedstack.masstransit.tests;

using System.Collections.Generic;
using OpenMedStack;
using OpenMedStack.Autofac.MassTransit;
using Xunit;

public class StaticTopicProviderTests
{
    [Fact]
    public void CanGetTopic()
    {
        var tenantProvider = new ConfigurationTenantProvider(new DeploymentConfiguration { TenantPrefix = "Test" });
        var provider = new StaticTopicProvider(tenantProvider);

        var topic = provider.GetTenantSpecific<TestEvent>();

        Assert.Equal("TestTest", topic);
    }

    [Fact]
    public void CanGetTopicForGenericType()
    {
        var tenantProvider = new ConfigurationTenantProvider(new DeploymentConfiguration { TenantPrefix = "Test" });
        var provider = new StaticTopicProvider(tenantProvider);

        var topic = provider.Get<List<string>>();

        Assert.Equal("System.Collections.Generic.List<System.String>", topic);
    }

    [Fact]
    public void CanGetTopicForDoubleGenericType()
    {
        var tenantProvider = new ConfigurationTenantProvider(new DeploymentConfiguration { TenantPrefix = "Test" });
        var provider = new StaticTopicProvider(tenantProvider);

        var topic = provider.Get<Dictionary<string, object>>();

        Assert.Equal("System.Collections.Generic.Dictionary<System.String,System.Object>", topic);
    }

    [Fact]
    public void CanGetCanonicalTopic()
    {
        var tenantProvider = new ConfigurationTenantProvider(new DeploymentConfiguration { TenantPrefix = "Test" });
        var provider = new StaticTopicProvider(tenantProvider);

        var topic = provider.Get<TestEvent>();

        Assert.Equal("Test", topic);
    }
}
