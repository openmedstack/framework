namespace OpenMedStack.Autofac.NEventstore.Domain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NEventStore;
    using NEventStore.Client;

    public interface ICommitDispatcher : IDisposable
    {
        Task<PollingClient2.HandlingResult> Dispatch(ICommit commit, CancellationToken cancellationToken);
    }
}