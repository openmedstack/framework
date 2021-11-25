namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    using System;
    using System.Net.Mime;
    using Newtonsoft.Json;

    public interface IConfigurator
    {
        IConfigurator WithContentType(ContentType contentType);
        IConfigurator Type<T>(string type);
        IConfigurator WithJsonOptions(Action<JsonSerializerSettings> options);
    }
}