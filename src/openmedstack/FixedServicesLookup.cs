// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixedServicesLookup.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the FixedServicesLookup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public class FixedServicesLookup : ILookupServices
{
    private readonly IDictionary<Regex, Uri> _serviceAddresses;

    public FixedServicesLookup(IEnumerable<KeyValuePair<Regex, Uri>> serviceAddresses)
    {
        _serviceAddresses = serviceAddresses.ToDictionary(x => x.Key, x => x.Value);
    }

    /// <inheritdoc />
    public Task<Uri> Lookup(Type type, CancellationToken cancellationToken = default)
    {
        var key = _serviceAddresses.Keys.First(x => x.IsMatch(type.FullName!));
        var address = _serviceAddresses[key];

        return Task.FromResult(address);
    }
}