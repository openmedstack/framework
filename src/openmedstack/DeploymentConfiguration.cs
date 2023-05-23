// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeploymentConfiguration.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the configuration for deployment.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// Defines the configuration for deployment.
/// </summary>
public class DeploymentConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeploymentConfiguration"/> class.
    /// </summary>
    public DeploymentConfiguration()
    {
        Timeout = TimeSpan.FromSeconds(30);
        RetryCount = 10;
        RetryInterval = TimeSpan.FromSeconds(1);
        Services = new Dictionary<Regex, Uri>();
        ValidIssuers = Array.Empty<string>();
    }

    /// <summary>
    /// Gets or sets the name of the application.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the environment name.
    /// </summary>
    /// <value>
    /// The environment.
    /// </value>
    public string Environment { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of available system services.
    /// </summary>
    public IDictionary<Regex, Uri> Services { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Uri"/> of the system service bus.
    /// </summary>
    public Uri? ServiceBus { get; set; }

    /// <summary>
    /// Gets or sets configured fail over hosts.
    /// </summary>
    public string[] ClusterHosts { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the username for the service bus.
    /// </summary>
    public string ServiceBusUsername { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password for the service bus.
    /// </summary>
    public string ServiceBusPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the queue to use on the service bus.
    /// </summary>
    public string QueueName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the connection string for accessing the application database.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the database provider.
    /// </summary>
    public string ProviderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timeout for the application initialization.
    /// </summary>
    public TimeSpan Timeout { get; set; }

    /// <summary>
    /// Gets or sets the tenant prefix.
    /// </summary>
    public string TenantPrefix { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the retry count.
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Gets or sets the retry interval.
    /// </summary>
    public TimeSpan RetryInterval { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Uri"/> of the token service.
    /// </summary>
    public string TokenService { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of <see cref="Uri"/> of the valid issuers.
    /// </summary>
    public string[] ValidIssuers { get; set; }

    /// <summary>
    /// Gets or sets the client id for the token service.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secret for client credentials.
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the scope for the client authentication.
    /// </summary>
    public string Scope { get; set; } = string.Empty;

    public IDictionary<string, string>? TopicMap { get; set; }

    /// <summary>
    /// Gets whether the defined database is an in-memory database.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>True if it is an in-memory database, otherwise false.</returns>
    public static bool IsInMemoryDatabase(string? connectionString) =>
        string.Equals(
            "inmemory",
            connectionString,
            StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets whether the defined database is an in-memory database.
    /// </summary>
    /// <returns>True if it is an in-memory database, otherwise false.</returns>
    public bool IsInMemoryDatabase() =>
        IsInMemoryDatabase(ConnectionString);

    /// <summary>
    /// Gets whether the defined service bus is an in-memory service bus.
    /// </summary>
    /// <returns>True if it is an in-memory service bus, otherwise false.</returns>
    public bool IsInMemoryServiceBus() => string.Equals(
        "loopback",
        ServiceBus?.Scheme,
        StringComparison.OrdinalIgnoreCase);
}