// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandBus.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the CommandBus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using global::MassTransit;
    using global::MassTransit.Configuration;
    using Microsoft.Extensions.Logging;
    using OpenMedStack.Commands;

    internal class CommandBus<TConfiguration> : IRouteCommands
        where TConfiguration : DeploymentConfiguration
    {
        private readonly IBus _bus;
        private readonly ILookupServices _lookup;
        private readonly ILogger<CommandBus<TConfiguration>> _logger;
        private readonly TConfiguration _configuration;

        public CommandBus(IBus bus, ILookupServices lookup, ILogger<CommandBus<TConfiguration>> logger, TConfiguration configuration)
        {
            _bus = bus;
            _lookup = lookup;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<CommandResponse> Send<T>(
            T command,
            IDictionary<string, object>? headers = null,
            CancellationToken cancellationToken = default)
            where T : DomainCommand
        {
            Task<bool> CommandCallback(SendContext<T> ctx)
            {
                if (headers != null)
                {
                    foreach (var (key, value) in headers)
                    {
                        ctx.Headers.Set(key, value);
                    }
                }
                return Task.FromResult(true);
            }


            var messageHeaders = new OpenMedStack.MessageHeaders(headers);
            try
            {
                var address = await _lookup.Lookup(typeof(T), cancellationToken).ConfigureAwait(false);
                var client = _bus.CreateRequestClient<T>(
                    address,
                    _configuration.Timeout);
                var handle = client.Create(command, cancellationToken, _configuration.Timeout);
                handle.AddPipeSpecification(
                    new ContextFilterPipeSpecification<SendContext<T>>(CommandCallback));
                var response = await handle.GetResponse<CommandResponse>().ConfigureAwait(false);

                return response.Message;
            }
            catch (RequestTimeoutException requestTimeoutException)
            {
                _logger.LogError(requestTimeoutException, requestTimeoutException.Message);
                if (messageHeaders.Expectation == ResponseExpectation.Always)
                {
                    throw;
                }

                return command.CreateResponse();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return command.CreateResponse(exception.Message);
            }
        }
    }
}
