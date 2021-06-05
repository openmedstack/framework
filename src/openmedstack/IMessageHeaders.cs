// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHeaders.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the message header interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the message header interface.
    /// </summary>
    public interface IMessageHeaders : IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>
        /// Gets the user token associated with the message.
        /// </summary>
        string? UserToken { get; }

        /// <summary>
        /// Gets the value of the sequence number heading.
        /// </summary>
        int SequenceNumber { get; }

        /// <summary>
        /// Gets the values in the message type header.
        /// </summary>
        string[] MessageType { get; }

        /// <summary>
        /// Gets the response expectation.
        /// </summary>
        ResponseExpectation Expectation { get; }

        /// <summary>
        /// Gets the requested header.
        /// </summary>
        /// <param name="key">The name of the header to retrieve.</param>
        /// <returns>The value of the header.</returns>
        object? this[string key] { get; }
    }
}