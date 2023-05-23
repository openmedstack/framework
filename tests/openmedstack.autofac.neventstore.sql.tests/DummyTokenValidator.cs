namespace OpenMedStack.Autofac.NEventStore.Sql.Tests;

using OpenMedStack;

internal class DummyTokenValidator : IValidateTokens
{
    /// <inheritdoc />
    public IdentityToken? Validate(string? token) => null;
}
