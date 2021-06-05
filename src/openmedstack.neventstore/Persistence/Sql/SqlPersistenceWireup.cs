// ReSharper disable once CheckNamespace
namespace NEventStore
{
    using System;
    using OpenMedStack.NEventStore;
    using OpenMedStack.NEventStore.Logging;
    using OpenMedStack.NEventStore.Persistence.Sql;
    using OpenMedStack.NEventStore.Serialization;

    public class SqlPersistenceWireup : PersistenceWireup
    {
        private const int DefaultPageSize = 512;
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(SqlPersistenceWireup));
        private int _pageSize = DefaultPageSize;

        public SqlPersistenceWireup(Wireup wireup, IConnectionFactory connectionFactory)
            : base(wireup)
        {
            Logger.Debug(PersistenceMessages.ConnectionFactorySpecified, connectionFactory);

            Logger.Verbose(PersistenceMessages.AutoDetectDialect);
            Container.Register<ISqlDialect>(c => null); // auto-detect
            Container.Register<IStreamIdHasher>(c => new Sha1StreamIdHasher());

            Container.Register(c => new SqlPersistenceFactory(
                connectionFactory,
                c.Resolve<ISerialize>() ?? throw new Exception($"Could not resolve {nameof(ISerialize)}"),
                c.Resolve<ISqlDialect>() ?? throw new Exception($"Could not resolve {nameof(ISqlDialect)}"),
                c.Resolve<IStreamIdHasher>() ?? throw new Exception($"Could not resolve {nameof(IStreamIdHasher)}"),
                _pageSize).Build());
        }

        public virtual SqlPersistenceWireup WithDialect(ISqlDialect instance)
        {
            Logger.Debug(PersistenceMessages.DialectSpecified, instance.GetType());
            Container.Register(instance);
            return this;
        }

        public virtual SqlPersistenceWireup PageEvery(int records)
        {
            Logger.Debug(PersistenceMessages.PagingSpecified, records);
            _pageSize = records;
            return this;
        }

        public virtual SqlPersistenceWireup WithStreamIdHasher(IStreamIdHasher instance)
        {
            Logger.Debug(PersistenceMessages.StreamIdHasherSpecified, instance.GetType());
            Container.Register(instance);
            return this;
        }

        public virtual SqlPersistenceWireup WithStreamIdHasher(Func<string, string> getStreamIdHash) => WithStreamIdHasher(new DelegateStreamIdHasher(getStreamIdHash));
    }
}