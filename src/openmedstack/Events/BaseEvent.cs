namespace OpenMedStack.Events
{
    using System;
    
    public abstract record BaseEvent : Message, ICorrelate
    {
        protected BaseEvent(string source, DateTimeOffset timeStamp, string? correlationId = null)
        :base(timeStamp)
        {
            if (timeStamp == DateTimeOffset.MinValue)
            {
                throw new ArgumentException(Strings.CannotUseMinTime, nameof(timeStamp));
            }

            Source = source;
            CorrelationId = correlationId;
        }

        /// <summary>
        ///     Gets the id of the source instance which raised the event.
        /// </summary>
        public string Source { get; }
        
        /// <inheritdoc />
        public string? CorrelationId { get; }
    }
}