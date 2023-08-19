namespace OpenMedStack;

public class ConfigurationTenantProvider : IProvideTenant
{
    private readonly string _tenantId;

    public ConfigurationTenantProvider(DeploymentConfiguration configuration)
    {
        _tenantId = configuration.TenantPrefix;
    }

    /// <inheritdoc />
    public string GetTenantName() => _tenantId;
}
