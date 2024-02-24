// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamoDbClientModule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the $TYPE$ type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.DynamoDb.Dispatcher
{
    using Amazon.DynamoDBv2;
    using Amazon.Runtime;
    using global::Autofac;
    using OpenMedStack.Autofac.NEventstore.Domain;
    using OpenMedStack.NEventStore.DynamoDbClient;

    internal class DynamoDbClientModule : Module
    {
        private readonly AWSCredentials _awsCredentials;
        private readonly AmazonDynamoDBStreamsConfig _streamsConfig;

        public DynamoDbClientModule(
            AWSCredentials awsCredentials,
            AmazonDynamoDBStreamsConfig streamsConfig)
        {
            _awsCredentials = awsCredentials;
            _streamsConfig = streamsConfig;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_awsCredentials).AsSelf().As<AWSCredentials>().SingleInstance().IfNotRegistered(typeof(AWSCredentials));
            builder.RegisterInstance(_streamsConfig).AsSelf().SingleInstance();
            builder.RegisterType<AmazonDynamoDBStreamsClient>().AsSelf().As<IAmazonDynamoDBStreams>().SingleInstance();
            builder.RegisterType<CompositeStreamClient>().As<StreamClient>().SingleInstance();
            builder.RegisterType<CompositeDynamoDbDispatcherSetup>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<EventCommitDispatcher>().As<ICommitDispatcher>().InstancePerDependency();
            builder.RegisterType<CommandCommitDispatcher<DeploymentConfiguration>>().As<ICommitDispatcher>().InstancePerDependency();
            builder.RegisterType<ReadModelCommitDispatcher>().As<ICommitDispatcher>().InstancePerDependency();
            builder.RegisterType<ReadModelUpdater>().As<IReadModelUpdater>().SingleInstance();        }
    }
}
