// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvironmentTopicProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the EnvironmentTopicProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit
{
    using System.Collections.Generic;
    using System.Reflection;

    internal class EnvironmentTopicProvider : IProvideTopic
    {
        private readonly IProvideTenant _tenantProvider;
        private readonly IDictionary<string, string> _topicMap;

        public EnvironmentTopicProvider(IProvideTenant tenantProvider, IDictionary<string, string>? topicMap = null)
        {
            _tenantProvider = tenantProvider;
            _topicMap = topicMap ?? new Dictionary<string, string>();
        }

        /// <inheritdoc />
        public string GetTenantSpecific<T>()
        {
            var tenant = _tenantProvider.GetTenantName();
            return tenant + Get<T>();
        }

        /// <inheritdoc />
        public string Get<T>()
        {
            var type = typeof(T);
            var fullName = type.FullName ?? "";
            if (_topicMap.TryGetValue(fullName, out var topic))
            {
                return topic;
            }

            var topicAttribute = type.GetCustomAttribute<TopicAttribute>();
            return topicAttribute != null ? topicAttribute.Topic : fullName;
        }
    }
}
