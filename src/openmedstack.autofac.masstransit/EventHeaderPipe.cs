// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventHeaderPipe.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the EventHeaderPipe type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;

namespace OpenMedStack.Autofac.MassTransit
{
    internal class EventHeaderPipe : IPipe<PublishContext>
    {
        private readonly IDictionary<string, object> _headers;

        public EventHeaderPipe(IDictionary<string, object>? headers)
        {
            _headers = headers ?? new Dictionary<string, object>();
        }

        public Task Send(PublishContext context)
        {
            foreach (var (key, value) in _headers)
            {
                context.Headers.Set(key, value);
            }

            return Task.CompletedTask;
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}