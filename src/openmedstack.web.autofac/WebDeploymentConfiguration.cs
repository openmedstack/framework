namespace OpenMedStack.Web.Autofac;

using System;

/// <summary>
/// Defines the web deployment configuration.
/// </summary>
public class WebDeploymentConfiguration : DeploymentConfiguration
{
    /// <summary>
    /// Gets or sets the urls to listen to.
    /// </summary>
    public string[] Urls { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the content root. If not set then the current directory will be used.
    /// </summary>
    public string? ContentRoot { get; set; }
}
