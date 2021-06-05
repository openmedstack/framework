namespace OpenMedStack.Autofac.NEventstore.Sql
{
    using System;
    using System.Data;
    using System.Data.Common;
    using NEventStore.Persistence;
    using NEventStore.Persistence.Sql;

    public class OpenMedStackConnectionFactory : IConnectionFactory
    {
        private readonly DbProviderFactory _providerFactory;
        private readonly string _connectionString;

        public OpenMedStackConnectionFactory(DbProviderFactory providerFactory, string connectionString)
        {
            _providerFactory = providerFactory;
            _connectionString = connectionString;
        }

        public Type GetDbProviderFactoryType() => _providerFactory.GetType();

        public IDbConnection Open() => Open(_connectionString);

        protected virtual IDbConnection Open(string connectionString) => OpenConnection(
            connectionString);

        protected virtual IDbConnection OpenConnection(string connectionString)
        {
            var factory = _providerFactory;
            var connection = factory.CreateConnection();
            if (connection == null)
            {
                throw new ConfigurationErrorsException("Bad Connection");
            }

            connection.ConnectionString = connectionString;

            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                throw new StorageUnavailableException(e.Message, e);
            }

            return connection;
        }
    }
}