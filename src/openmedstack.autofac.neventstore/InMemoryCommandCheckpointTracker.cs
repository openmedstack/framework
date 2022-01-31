namespace OpenMedStack.Autofac.NEventstore;

using OpenMedStack.Domain;

public class InMemoryCommandCheckpointTracker : InMemoryCheckpointTracker, ITrackCommandCheckpoints
{
}