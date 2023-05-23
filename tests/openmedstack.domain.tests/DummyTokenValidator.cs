namespace OpenMedStack.Domain.Tests;

using OpenMedStack;

internal class DummyTokenValidator : IValidateTokens
{
    /// <inheritdoc />
    public IdentityToken? Validate(string? token) => null;
}
