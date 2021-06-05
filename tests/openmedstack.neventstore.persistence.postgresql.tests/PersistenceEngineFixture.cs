// ReSharper disable once CheckNamespace
namespace OpenMedStack.NEventStore.Persistence.AcceptanceTests
{
    using OpenMedStack.NEventStore.Persistence.Sql;
    using OpenMedStack.NEventStore.Persistence.Sql.SqlDialects;
    using OpenMedStack.NEventStore.Serialization;
    using Npgsql;

    public partial class PersistenceEngineFixture
    {
        public PersistenceEngineFixture()
        {
            _createPersistence = pageSize =>
                new SqlPersistenceFactory(
                    new NetStandardConnectionFactory(
                        NpgsqlFactory.Instance,
                        "Server=127.0.0.1;Keepalive=1;Pooling=true;MinPoolSize=1;MaxPoolSize=20;Port=5432;Database=eventdb;User Id=openmedstack;Password=openmedstack;"),
                    new NesJsonSerializer(),
                    new PostgreSqlDialect(),
                    pageSize: pageSize).Build();
        }
    }
}