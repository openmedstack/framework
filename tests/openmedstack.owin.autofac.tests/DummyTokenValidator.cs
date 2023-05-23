namespace OpenMedStack.Web.Autofac.Tests;

internal class DummyTokenValidator : IValidateTokens
{
    /// <inheritdoc />
    public IdentityToken? Validate(string? token) => null;
}