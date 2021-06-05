namespace OpenMedStack.Autofac.NEventStore.Sql.Tests
{
    using OpenMedStack;

    internal class TestTenantProvider : IProvideTenant
    {
        /// <inheritdoc />
        public string GetTenantName() => "test";
    }
}