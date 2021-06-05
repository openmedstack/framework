namespace OpenMedStack.Autofac.NEventstore.Domain
{
    using System.Threading.Tasks;
    using NEventStore;

    public interface IReadModelUpdater
    {
        Task<bool> Update(ICommit commit);
    }
}