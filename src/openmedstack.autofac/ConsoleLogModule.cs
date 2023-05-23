namespace OpenMedStack.Autofac;

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using global::Autofac;
using global::Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class ConsoleLogModule : Module
{
    private readonly bool _enableConsoleLogger;
    private readonly (string, LogLevel)[]? _filters;

    public ConsoleLogModule(bool enableConsoleLogger, (string, LogLevel)[]? filters)
    {
        _enableConsoleLogger = enableConsoleLogger;
        _filters = filters;
    }

    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        var collection = new InternalServiceCollection();
        collection.AddLogging(
            x =>
            {
                if (_filters != null)
                {
                    x = _filters.Aggregate(x, (b, filter) => b.AddFilter(filter.Item1, filter.Item2));
                }
                if (_enableConsoleLogger)
                {
                    x.AddJsonConsole(
                        c =>
                        {
                            c.UseUtcTimestamp = true;
                            c.JsonWriterOptions = new JsonWriterOptions
                            {
                                Indented = false
                            };
                            c.IncludeScopes = true;
                        });
                }
            });
        builder.Populate(collection);
    }

    private class InternalServiceCollection : List<ServiceDescriptor>, IServiceCollection { }
}