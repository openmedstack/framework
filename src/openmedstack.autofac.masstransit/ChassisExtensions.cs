namespace OpenMedStack.Autofac.MassTransit
{
    public static class ChassisExtensions
    {
        public static Chassis UsingInMemoryMassTransit(this Chassis chassis)
        {
            chassis.AddAutofacModules(
                (c, a) => new DomainModule(c, a),
                (c, _) => new InMemoryMassTransitModule(c));

            return chassis;
        }
    }
}