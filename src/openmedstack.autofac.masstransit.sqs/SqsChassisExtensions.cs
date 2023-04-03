namespace OpenMedStack.Autofac.MassTransit.Sqs
{
    public static class SqsChassisExtensions
    {
        public static Chassis<TConfiguration> UsingMassTransitOverSqs<TConfiguration>(
            this Chassis<TConfiguration> chassis)
            where TConfiguration : DeploymentConfiguration
        {
            chassis.AddAutofacModules(
                (c, a) => new DomainModule<TConfiguration>(c, a),
                (c, _) => new SqsMassTransitModule<TConfiguration>(c));

            return chassis;
        }
    }
}
