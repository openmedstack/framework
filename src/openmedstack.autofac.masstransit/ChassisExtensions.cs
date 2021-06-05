namespace OpenMedStack.Autofac.MassTransit
{
    public static class ChassisExtensions
    {
        public static Chassis UsingInMemoryMassTransit(this Chassis chassis)
        {
            chassis.AddAutofacModules(
                (c, a) => new DomainModule(a),
                (c, a) => new InMemoryMassTransitModule(c));

            return chassis;
        }
    }
}