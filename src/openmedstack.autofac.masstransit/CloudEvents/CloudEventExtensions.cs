// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudEventExtensions.cs" company="Reimers.dk">
//   Copyright � Reimers.dk
// </copyright>
// <summary>
//   Defines the CloudEventExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit.CloudEvents;

using System;
using CloudNative.CloudEvents;

internal static class CloudEventExtensions
{
    public static Uri? GetUri(this CloudEvent cloudEvent, string attribute)
    {
        var value = cloudEvent[attribute];
        return value == null ? null : new Uri(value.ToString()!, UriKind.RelativeOrAbsolute);
    }
        
    public static Guid? GetGuid(this CloudEvent cloudEvent, string attribute)
    {
        var value = cloudEvent[attribute];
        return value == null ? default : Guid.Parse(value.ToString()!);
    }
}
