using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.EFModels.Entities;
using Microsoft.EntityFrameworkCore;

namespace Qanat.Swagger.Filters
{
    public class ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        QanatDbContext dbContext)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
    {
        public const string ApiKeyName = "x-api-key";

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyName, out var extractedApiKey))
            {
                return AuthenticateResult.Fail("Api Key was not provided");
            }

            if (!Guid.TryParse(extractedApiKey, out var parsedApiKey))
            {
                return AuthenticateResult.Fail("Api Key is not valid");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.ApiKey == parsedApiKey);
            if (user == null)
            {
                return AuthenticateResult.Fail("Api Key is not valid");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
                new Claim("RoleID", user.RoleID.ToString())
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
