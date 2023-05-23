namespace OpenMedStack.Framework.IntegrationTests;

using OpenMedStack;

internal class TestTenantProvider : IProvideTenant
{
    /// <inheritdoc />
    public string GetTenantName() => "test";
}