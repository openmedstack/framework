namespace OpenMedStack.Autofac.MassTransit.Sqs
{
    using System;
    using global::MassTransit.Transports;

    public class SegmentedMessageNameFormatter : IMessageNameFormatter
    {
        private readonly string _segment;
        private readonly IMessageNameFormatter _innerFormatter;

        public SegmentedMessageNameFormatter(string segment, IMessageNameFormatter innerFormatter)
        {
            _segment = segment;
            _innerFormatter = innerFormatter;
        }

        /// <inheritdoc />
        public MessageName GetMessageName(Type type) =>
            string.IsNullOrWhiteSpace(_segment)
                ? _innerFormatter.GetMessageName(type)
                : new MessageName($"{_segment}_{_innerFormatter.GetMessageName(type)}");
    }
}