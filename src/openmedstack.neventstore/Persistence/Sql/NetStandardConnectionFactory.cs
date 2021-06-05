namespace OpenMedStack.NEventStore.Persistence.Sql
{
    using System;
    using System.Data;
    using System.Data.Common;
    using Logging;
    using Persistence;

    public class NetStandardConnectionFactory : IConnectionFactory
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(NetStandardConnectionFactory));

        private readonly DbProviderFactory _providerFactory;
        private readonly string _connectionString;

        public NetStandardConnectionFactory(DbProviderFactory providerFactory, string connectionString)
        {
            _providerFactory = providerFactory;
            _connectionString = connectionString;
        }

        public Type GetDbProviderFactoryType() => _providerFactory.GetType();

        public IDbConnection Open()
        {
            Logger.Verbose(PersistenceMessages.OpeningMasterConnection, _connectionString);
            return Open(_connectionString);
        }

        protected virtual IDbConnection Open(string connectionString)
        {
            return new ConnectionScope(connectionString, () => OpenConnection(connectionString));
        }

        protected virtual IDbConnection OpenConnection(string connectionString)
        {
            var factory = _providerFactory;
            var connection = factory.CreateConnection();
            if (connection == null)
            {
                throw new ConfigurationErrorsException(PersistenceMessages.BadConnectionName);
            }

            connection.ConnectionString = connectionString;

            try
            {
                Logger.Verbose(PersistenceMessages.OpeningConnection, connectionString);
                connection.Open();
            }
            catch (Exception e)
            {
                Logger.Warn(PersistenceMessages.OpenFailed, connectionString);
                throw new StorageUnavailableException(e.Message, e);
            }

            return connection;
        }
    }
}
