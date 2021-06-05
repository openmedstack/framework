namespace OpenMedStack.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRepository
    {
        Task<TAggregate> GetById<TAggregate>(string id, CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregate;

        Task<TAggregate> GetById<TAggregate>(string id, int version, CancellationToken cancellationToken = default)
            where TAggregate : class, IAggregate;

        Task Save(
            IAggregate aggregate,
            Action<IDictionary<string, object>>? updateHeaders = null,
            CancellationToken cancellationToken = default);
    }
}
