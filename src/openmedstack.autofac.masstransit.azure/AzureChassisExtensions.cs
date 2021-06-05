namespace OpenMedStack.Autofac.MassTransit.Azure
{
    public static class AzureChassisExtensions
    {
        public static Chassis UsingMassTransitOverAzure(this Chassis chassis)
        {
            chassis.AddAutofacModules(
                (c, a) => new DomainModule(a),
                (c, a) => new AzureServiceBusMassTransitModule(c));

            return chassis;
        }
    }
}
