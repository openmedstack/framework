namespace OpenMedStack.Autofac.MassTransit.Azure
{
    public static class AzureChassisExtensions
    {
        public static Chassis<TConfiguration> UsingMassTransitOverAzure<TConfiguration>(
            this Chassis<TConfiguration> chassis)
            where TConfiguration : DeploymentConfiguration
        {
            chassis.AddAutofacModules(
                (c, a) => new DomainModule<TConfiguration>(c, a),
                (c, _) => new AzureServiceBusMassTransitModule<TConfiguration>(c));

            return chassis;
        }
    }
}
