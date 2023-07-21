namespace OpenMedStack.Web.Autofac.Tests.Steps;

using System;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

public partial class FeatureSteps
{
    [When(@"requesting application root")]
    public async Task WhenRequestingApplicationRoot()
    {
        await _client.GetAsync("http://localhost").ConfigureAwait(false);
    }

    [Then(@"event is published on bus")]
    public void ThenEventIsPublishedOnBus()
    {
        var success = _waitHandle.Wait(TimeSpan.FromSeconds(3));
        Assert.True(success);
    }

    [When(@"requesting command path")]
    public async Task WhenRequestingCommandPath()
    {
        var response = await _client.GetAsync("http://localhost/commands").ConfigureAwait(false);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
