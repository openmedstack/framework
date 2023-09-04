// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonDeploymentConfigurationLoader.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the JsonDeploymentConfigurationLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Infrastructure.Bootstrapping
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Linq;
    using Json;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the static json configuration loader.
    /// </summary>
    public static class JsonDeploymentConfigurationLoader
    {
        /// <summary>
        /// Loads the passed configuration file.
        /// </summary>
        /// <param name="configFile">The path to the file to load.</param>
        /// <typeparam name="T">The <see cref="Type"/> of configuration file to load.</typeparam>
        /// <returns>The loaded configuration file as a <typeparamref name="T"/>.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist at the passed location.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the configuration is not valid.</exception>
        /// <para>The passed configuration file can be updated by passing in environment values. Keys with the format {{key}} will be replaced with the environment value with the same key.</para>
        public static T Load<T>(this string configFile)
            where T : DeploymentConfiguration, new()
        {
            var filePath = configFile.ToApplicationPath();
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Could not find configuration file at {filePath}");
            }

            var json = Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .Aggregate(
                    File.ReadAllText(filePath),
                    (current, variable) => current.Replace($"{{{variable.Key}}}", variable.Value!.ToString()));
            var configuration = JsonConvert.DeserializeObject<T>(
                json,
                new ServiceDictionaryConverter(),
                new IpAddressConverter());

            if(configuration == null)
            {
                throw new InvalidOperationException("Cannot deserialize input");
            }

            return configuration;
        }
    }
}
