namespace OpenMedStack.NEventStore.Persistence.Sql
{
    using System;
    using Persistence;
    using Serialization;

    public class SqlPersistenceFactory : IPersistenceFactory
    {
        private const int DefaultPageSize = 128;

        public SqlPersistenceFactory(
            IConnectionFactory factory,
            ISerialize serializer,
            ISqlDialect dialect,
            IStreamIdHasher? streamIdHasher = null,
            int pageSize = DefaultPageSize)
            : this(serializer, streamIdHasher ?? new Sha1StreamIdHasher(), pageSize)
        {
            ConnectionFactory = factory;
            Dialect = dialect ?? throw new ArgumentNullException(nameof(dialect));
        }

        private SqlPersistenceFactory(ISerialize serializer, IStreamIdHasher streamIdHasher, int pageSize)
        {
            Serializer = serializer;
            StreamIdHasher = streamIdHasher;
            PageSize = pageSize;
        }

        protected virtual IConnectionFactory? ConnectionFactory { get; }

        protected virtual ISqlDialect? Dialect { get; }

        protected virtual ISerialize Serializer { get; }

        protected virtual IStreamIdHasher StreamIdHasher { get; }

        protected int PageSize { get; set; }

        public virtual IPersistStreams Build() => new SqlPersistenceEngine(ConnectionFactory, Dialect, Serializer, PageSize, StreamIdHasher);
    }
}