namespace OpenMedStack.Autofac.MassTransit.ActiveMq
{
    public static class ChassisExtensions
    {
        public static Chassis UsingMassTransitOverActiveMq(this Chassis chassis)
        {
            chassis.AddAutofacModules(
                (c, a) => new DomainModule(c, a),
                (c, a) => new ActiveMqMassTransitModule(c));

            return chassis;
        }
    }
}
