namespace OpenMedStack.Domain
{
    public interface IMemento
    {
        string Id { get; set; }

        int Version { get; set; }
    }
}