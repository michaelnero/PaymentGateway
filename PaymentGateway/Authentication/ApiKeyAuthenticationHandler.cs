using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PaymentGateway.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string _apiKeyHeaderName = "X-Api-Key";

        private readonly IApiKeyStore _store;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IApiKeyStore store)
            : base(options, logger, encoder, clock)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(_apiKeyHeaderName, out var apiKeyHeaderValues) || (0 == apiKeyHeaderValues.Count))
            {
                return NoKeyProvided();
            }

            var providedApiKey = apiKeyHeaderValues.SingleOrDefault();
            if (providedApiKey is null)
            {
                return NoKeyProvided();
            }

            var entity = await _store.FindAsync(providedApiKey);

            if (true == (entity?.IsActive ?? false))
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, entity.Owner)
                };

                var principal = new ClaimsPrincipal(new List<ClaimsIdentity>
                {
                    new ClaimsIdentity(claims, Options.AuthenticationType)
                });
                
                var ticket = new AuthenticationTicket(principal, Options.Scheme);

                return AuthenticateResult.Success(ticket);
            }

            return InvalidKeyProvided();
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;

            return Task.CompletedTask;
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;

            return Task.CompletedTask;
        }

        private static AuthenticateResult NoKeyProvided() => AuthenticateResult.NoResult();

        private static AuthenticateResult InvalidKeyProvided() => AuthenticateResult.Fail("Invalid API Key provided.");
    }
}
