namespace OpenMedStack.Autofac.MassTransit.Sqs
{
    public static class SqsChassisExtensions
    {
        public static Chassis UsingMassTransitOverSqs(this Chassis chassis)
        {
            chassis.AddAutofacModules(
                (c, a) => new DomainModule(c, a),
                (c, a) => new SqsMassTransitModule(c));

            return chassis;
        }
    }
}
