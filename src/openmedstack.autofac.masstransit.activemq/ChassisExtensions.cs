namespace OpenMedStack.Autofac.MassTransit.ActiveMq;

public static class ChassisExtensions
{
    public static Chassis<TConfiguration> UsingMassTransitOverActiveMq<TConfiguration>(this Chassis<TConfiguration> chassis)
        where TConfiguration : DeploymentConfiguration
    {
        chassis.AddAutofacModules(
            (c, a) => new DomainModule<TConfiguration>(c, a),
            (c, _) => new ActiveMqMassTransitModule<TConfiguration>(c));

        return chassis;
    }
}
