// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamoDbChassisExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the DynamoDbChassisExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.DynamoDb.Dispatcher;

using System;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using OpenMedStack;
using OpenMedStack.Autofac;

public static class DynamoDbChassisExtensions
{
    public static Chassis<TConfiguration> UsingDynamoDbDispatcher<TConfiguration>(
        this Chassis<TConfiguration> chassis,
        AWSCredentials credentials,
        AmazonDynamoDBStreamsConfig streamsConfig)
        where TConfiguration : DeploymentConfiguration
    {
        return chassis.AddAutofacModules((_, __) => new DynamoDbClientModule(credentials, streamsConfig));
    }

    public static Chassis<TConfiguration> UsingDynamoDbDispatcher<TConfiguration>(
        this Chassis<TConfiguration> chassis,
        AWSCredentials credentials,
        Uri serviceUrl)
        where TConfiguration : DeploymentConfiguration
    {
        return chassis.AddAutofacModules(
            (_, __) => new DynamoDbClientModule(
                credentials,
                new AmazonDynamoDBStreamsConfig
                {
                    AllowAutoRedirect = true,
                    ServiceURL = serviceUrl.AbsoluteUri,
                    UseHttp = serviceUrl.Scheme == Uri.UriSchemeHttps
                }));
    }
}
