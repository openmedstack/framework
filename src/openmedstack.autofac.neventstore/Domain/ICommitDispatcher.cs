namespace OpenMedStack.Autofac.NEventstore.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using OpenMedStack.NEventStore.Abstractions;

public interface ICommitDispatcher : IDisposable
{
    Task<HandlingResult> Dispatch(ICommit commit, CancellationToken cancellationToken);
}
