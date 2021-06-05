// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityToken.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the generic identity token.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack
{
    using System;
    using System.Security.Claims;

    /// <summary>
    /// Defines the generic identity token.
    /// </summary>
    public class IdentityToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityToken"/> class.
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="accessToken"></param>
        /// <param name="tokenType"></param>
        /// <param name="expires"></param>
        /// <param name="scope"></param>
        public IdentityToken(ClaimsPrincipal principal, string accessToken, string tokenType, DateTime expires, string scope)
        {
            Principal = principal;
            AccessToken = accessToken;
            TokenType = tokenType;
            Issued = DateTime.UtcNow;
            Expires = expires;
            Scope = scope;
        }

        /// <summary>
        /// Gets the <see cref="ClaimsPrincipal"/> represented by the token.
        /// </summary>
        public ClaimsPrincipal Principal { get; }

        /// <summary>
        /// Gets or sets the access token issued by the authorization server.
        /// </summary>
        public string AccessToken { get; }

        /// <summary>
        /// Gets the expiry time for the token.
        /// </summary>
        public DateTimeOffset Expires { get; }

        /// <summary>
        /// Gets or sets the token type as specified in http://tools.ietf.org/html/rfc6749#section-7.1.
        /// </summary>
        public string TokenType { get; }


        /// <summary>
        /// Gets or sets the scope of the access token as specified in http://tools.ietf.org/html/rfc6749#section-3.3.
        /// </summary>
        public string Scope { get; }

        /// <summary>
        /// The date and time that this token was issued.
        /// <remarks>
        /// It should be set by the CLIENT after the token was received from the server.
        /// </remarks>
        /// </summary>
        public DateTime Issued { get; }
    }
}
