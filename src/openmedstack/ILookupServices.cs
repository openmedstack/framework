// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILookupServices.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the service lookup interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the service lookup interface.
    /// </summary>
    public interface ILookupServices
    {
        /// <summary>
        /// Looks up the given service.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of service to look up.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/> encapsulating the lookup operation.</returns>
        Task<Uri> Lookup(Type type, CancellationToken cancellationToken = default);
    }
}