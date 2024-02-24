// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamoDbChassisExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the DynamoDbChassisExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.DynamoDb;

using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using OpenMedStack;
using OpenMedStack.Autofac;

public static class DynamoDbChassisExtensions
{
    public static Chassis<TConfiguration> UsingDynamoDbEventStore<TConfiguration>(
        this Chassis<TConfiguration> chassis,
        AWSCredentials credentials,
        RegionEndpoint region)
        where TConfiguration : DeploymentConfiguration
    {
        return chassis.AddAutofacModules(
            (_, __) => new DynamoDbEventStoreModule(credentials, new AmazonDynamoDBConfig { RegionEndpoint = region }));
    }

    public static Chassis<TConfiguration> UsingDynamoDbEventStore<TConfiguration>(
        this Chassis<TConfiguration> chassis,
        AWSCredentials credentials,
        AmazonDynamoDBConfig config)
        where TConfiguration : DeploymentConfiguration
    {
        return chassis.AddAutofacModules((_, __) => new DynamoDbEventStoreModule(credentials, config));
    }

    public static Chassis<TConfiguration> UsingDynamoDbEventStore<TConfiguration>(
        this Chassis<TConfiguration> chassis,
        AWSCredentials credentials,
        Uri serviceUrl)
        where TConfiguration : DeploymentConfiguration =>
        UsingDynamoDbEventStore(
            chassis,
            credentials,
            new AmazonDynamoDBConfig
            {
                AllowAutoRedirect = true,
                ServiceURL = serviceUrl.AbsoluteUri,
                UseHttp = serviceUrl.Scheme == Uri.UriSchemeHttps
            });
}
