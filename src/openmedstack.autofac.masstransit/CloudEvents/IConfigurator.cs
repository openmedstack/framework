namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    using System.Net.Mime;

    public interface IConfigurator
    {
        IConfigurator WithContentType(ContentType contentType);
    }
}