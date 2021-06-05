namespace OpenMedStack.Web.Autofac.Tests
{
    internal class TestTenantProvider : IProvideTenant
    {
        /// <inheritdoc />
        public string GetTenantName() => "test";
    }
}