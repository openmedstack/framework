namespace OpenMedStack.Autofac.MassTransit
{
    public static class ChassisExtensions
    {
        public static Chassis<TConfiguration> UsingInMemoryMassTransit<TConfiguration>(
            this Chassis<TConfiguration> chassis)
            where TConfiguration : DeploymentConfiguration
        {
            chassis.AddAutofacModules(
                (c, a) => new DomainModule<TConfiguration>(c, a),
                (c, _) => new InMemoryMassTransitModule<TConfiguration>(c));

            return chassis;
        }
    }
}
