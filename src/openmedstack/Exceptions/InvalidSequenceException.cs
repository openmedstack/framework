// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidSequenceException.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the exception for out of order messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Exceptions
{
    using System;

    /// <summary>
    /// Defines the exception for out of order messages.
    /// </summary>
    public class InvalidSequenceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSequenceException"/> class.
        /// </summary>
        /// <param name="sequenceNumber">The actual sequence number received.</param>
        /// <param name="expected">The expected sequence number.</param>
        public InvalidSequenceException(int sequenceNumber, int expected)
            : base($"Sequence number {sequenceNumber} is different from expected {expected}.")
        {
        }
    }
}
