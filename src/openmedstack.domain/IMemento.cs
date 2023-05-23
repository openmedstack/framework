namespace OpenMedStack.Domain;

public interface IMemento
{
    string Id { get; }

    int Version { get; }
}
