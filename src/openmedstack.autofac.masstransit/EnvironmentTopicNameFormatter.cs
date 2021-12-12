// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvironmentTopicNameFormatter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the EnvironmentTopicNameFormatter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit
{
    using global::MassTransit.Topology;

    internal class EnvironmentTopicNameFormatter : IEntityNameFormatter
    {
        private readonly IProvideTopic _topicProvider;

        public EnvironmentTopicNameFormatter(IProvideTopic topicProvider)
        {
            _topicProvider = topicProvider;
        }

        /// <inheritdoc />
        public string FormatEntityName<T>() => _topicProvider.GetTenantSpecific<T>();
    }
}
