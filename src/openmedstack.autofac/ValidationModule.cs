namespace OpenMedStack.Autofac;

using global::Autofac;
using OpenMedStack.Startup;

internal class ValidationModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<EventHandlerCreationValidator>().As<IValidateStartup>();
        builder.RegisterType<CommandHandlerCreationValidator>().As<IValidateStartup>();
        builder.RegisterType<CommandHandlerRoutingValidator>().As<IValidateStartup>();
        builder.RegisterType<CommandHandlerRegistrationValidator>().As<IValidateStartup>();
        builder.RegisterType<EventSubscriberValidator>().As<IValidateStartup>();
    }
}
