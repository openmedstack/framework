// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudEventTopicAttribute.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the CloudEventTopicAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    using System;

    public class CloudEventTopicAttribute : Attribute
    {
        public CloudEventTopicAttribute(string topic)
        {
            Topic = topic;
        }
    
        public string Topic { get; }
    }
}
