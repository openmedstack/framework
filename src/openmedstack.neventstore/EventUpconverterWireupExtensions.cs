namespace OpenMedStack.NEventStore
{
    public static class EventUpconverterWireupExtensions
    {
        public static EventUpconverterWireup UsingEventUpconversion(this Wireup wireup) => new(wireup);
    }
}