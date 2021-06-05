using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMedStack.Domain
{
    using System.Threading;

    public interface ISagaRepository
    {
        Task<TSaga> GetById<TSaga>(string sagaId) where TSaga : class, ISaga;

        Task Save(ISaga saga, Action<IDictionary<string, object>>? updateHeaders = null, CancellationToken cancellationToken = default);
    }
}