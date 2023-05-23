// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHeaders.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the concrete implementation of a message header container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines the concrete implementation of a message header container.
/// </summary>
public class MessageHeaders : IMessageHeaders
{
    private const string SequenceNumberKey = "SequenceNumber";
    private const string TokenKey = "token";
    private const string MessageTypeKey = "messageType";
    private const string ExpectationKey = "expectation";

    private readonly Dictionary<string, object> _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageHeaders"/> class.
    /// </summary>
    /// <param name="items">The headers and values to include.</param>
    public MessageHeaders(params KeyValuePair<string, object>[] items)
    {
        _items = new Dictionary<string, object>(items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageHeaders"/> class.
    /// </summary>
    /// <param name="items">The headers and values to include.</param>
    public MessageHeaders(IEnumerable<KeyValuePair<string, object>>? items)
        : this((items ?? Enumerable.Empty<KeyValuePair<string, object>>()).ToArray())
    {
    }

    public ResponseExpectation Expectation =>
        _items.TryGetValue(ExpectationKey, out var expectation) ? (ResponseExpectation)expectation : ResponseExpectation.Always;

    /// <inheritdoc />
    public int SequenceNumber => _items.ContainsKey(SequenceNumberKey)
        ? (int)_items[SequenceNumberKey]
        : 0;

    /// <inheritdoc />
    public string? UserToken => _items.ContainsKey(TokenKey)
        ? _items[TokenKey] as string
        : null;

    /// <inheritdoc />
    public string[] MessageType => _items[MessageTypeKey] as string[] ?? Array.Empty<string>();

    /// <inheritdoc />
    public object? this[string key] => _items.ContainsKey(key) ? _items[key] : null;

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
