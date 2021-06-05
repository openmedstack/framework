namespace OpenMedStack.Framework.IntegrationTests
{
    using OpenMedStack;

    internal class DummyTokenValidator : IValidateTokens
    {
        /// <inheritdoc />
        public IdentityToken? Validate(string? token) => null;
    }
}