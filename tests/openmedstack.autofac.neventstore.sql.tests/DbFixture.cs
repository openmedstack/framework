namespace OpenMedStack.Autofac.NEventStore.Sql.Tests;

using System.Threading;
using System.Threading.Tasks;
using OpenMedStack;
using OpenMedStack.NEventStore.Abstractions;

internal class DbFixture : IBootstrapSystem
{
    private readonly IManagePersistence _persistence;

    public DbFixture(IManagePersistence persistence)
    {
        _persistence = persistence;
    }

    /// <inheritdoc />
    public uint Order { get; } = 1;

    /// <inheritdoc />
    public Task Setup(CancellationToken cancellationToken) => _persistence.Initialize();

    /// <inheritdoc />
    public Task Shutdown(CancellationToken cancellationToken) => _persistence.Drop();
}
