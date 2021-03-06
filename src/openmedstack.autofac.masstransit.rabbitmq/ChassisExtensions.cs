namespace OpenMedStack.Autofac.MassTransit.RabbitMq
{
    public static class ChassisExtensions
    {
        public static Chassis UsingMassTransitOverRabbitMq(this Chassis chassis)
        {
            chassis.AddAutofacModules(
                (c, a) => new DomainModule(c, a),
                (c, _) => new RabbitMqMassTransitModule(c));

            return chassis;
        }
    }
}
