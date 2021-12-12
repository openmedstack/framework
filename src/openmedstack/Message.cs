// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Message.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the Message type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack
{
    using System;

    /// <summary>
    /// The abstract message definition
    /// </summary>
    public abstract class Message
    {
        protected Message(DateTimeOffset timestamp)
        {
            Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the time stamp of the message.
        /// </summary>
        public DateTimeOffset Timestamp { get; }
    }
}
