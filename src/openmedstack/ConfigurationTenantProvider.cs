namespace OpenMedStack
{
    public class ConfigurationTenantProvider : IProvideTenant
    {
        private readonly string _tenantId;

        public ConfigurationTenantProvider(DeploymentConfiguration configuration)
        {
            _tenantId = $"{configuration.TenantPrefix}{configuration.QueueName}";
        }

        /// <inheritdoc />
        public string GetTenantName() => _tenantId;
    }
}
