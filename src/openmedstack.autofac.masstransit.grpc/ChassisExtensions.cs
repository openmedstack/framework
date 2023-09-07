namespace OpenMedStack.Autofac.MassTransit.Grpc
{
    using OpenMedStack;
    using OpenMedStack.Autofac;

    public static class ChassisExtensions
    {
        public static Chassis<TConfiguration> UsingMassTransitOverGrpc<TConfiguration>(
            this Chassis<TConfiguration> chassis)
            where TConfiguration : DeploymentConfiguration
        {
            chassis.AddAutofacModules(
                (c, a) => new DomainModule<TConfiguration>(c, a),
                (c, _) => new GrpcMassTransitModule<TConfiguration>(c));

            return chassis;
        }
    }
}
