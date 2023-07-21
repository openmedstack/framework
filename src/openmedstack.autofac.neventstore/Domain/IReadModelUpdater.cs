namespace OpenMedStack.Autofac.NEventstore.Domain;

using System.Threading.Tasks;
using OpenMedStack.NEventStore.Abstractions;

public interface IReadModelUpdater
{
    Task<bool> Update(ICommit commit);
}
