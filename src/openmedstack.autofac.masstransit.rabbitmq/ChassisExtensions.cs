namespace OpenMedStack.Autofac.MassTransit.RabbitMq
{
    public static class ChassisExtensions
    {
        public static Chassis<TConfiguration> UsingMassTransitOverRabbitMq<TConfiguration>(
            this Chassis<TConfiguration> chassis)
            where TConfiguration : DeploymentConfiguration
        {
            chassis.AddAutofacModules(
                (c, a) => new DomainModule<TConfiguration>(c, a),
                (c, _) => new RabbitMqMassTransitModule<TConfiguration>(c));

            return chassis;
        }
    }
}
