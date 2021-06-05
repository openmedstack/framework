namespace OpenMedStack.Events
{
    using System;

    public abstract class BaseEvent : ICorrelate, IEquatable<BaseEvent>
    {
        protected BaseEvent(string source, DateTimeOffset timeStamp, string? correlationId = null)
        {
            if (timeStamp == DateTimeOffset.MinValue)
            {
                throw new ArgumentException("Cannot use min time", nameof(timeStamp));
            }

            Source = source;
            TimeStamp = timeStamp;
            CorrelationId = correlationId;
        }

        /// <summary>
        ///     Gets the id of the source instance which raised the event.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets the time stamp for the event.
        /// </summary>
        public DateTimeOffset TimeStamp { get; }

        /// <inheritdoc />
        public string? CorrelationId { get; }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Source, TimeStamp, CorrelationId);

        /// <inheritdoc />
        public bool Equals(BaseEvent? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.GetType() == GetType() && Source == other.Source && TimeStamp.Equals(other.TimeStamp) && CorrelationId == other.CorrelationId;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            return ReferenceEquals(this, obj) || Equals((BaseEvent)obj);
        }

        public static bool operator ==(BaseEvent left, BaseEvent right) => Equals(left, right);

        public static bool operator !=(BaseEvent left, BaseEvent right) => !Equals(left, right);
    }
}