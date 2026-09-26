using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using EncryptionOracleVulnerability.Controllers;
using EncryptionOracleVulnerability.Utilities.Interfaces;

namespace EncryptionOracleVulnerability.Middlewares
{
    public class AuthTokenMiddleware
    {
        public const string AuthenticationType = "EncryptedToken";
        public const string AnonymousUserName = "Anonymous";

        private readonly RequestDelegate _next;
        private readonly IEncryptionUtility _encryptionUtility;
        private readonly ILogger<AuthTokenMiddleware> _logger;
        private readonly string _password;
        private readonly byte[] _salt;

        public AuthTokenMiddleware(
            RequestDelegate next,
            IEncryptionUtility encryptionUtility,
            IConfiguration configuration,
            ILogger<AuthTokenMiddleware> logger)
        {
            _next = next;
            _encryptionUtility = encryptionUtility;
            _logger = logger;
            _password = configuration["TokenEncryption:Password"]
                ?? throw new InvalidOperationException("TokenEncryption:Password is not configured.");
            _salt = Convert.FromBase64String(configuration["TokenEncryption:Salt"]
                ?? throw new InvalidOperationException("TokenEncryption:Salt is not configured."));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.User = CreateAnonymousPrincipal();

            if (context.Request.Cookies.TryGetValue(LoginController.AuthTokenCookieName, out string? token) &&
                !string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    byte[] encrypted = Convert.FromBase64String(token.Trim());
                    byte[] decrypted = _encryptionUtility.DecryptDataAesGcm(encrypted, _password, _salt);

                    var tokenData = JsonSerializer.Deserialize<AuthTokenData>(decrypted);
                    if (tokenData is not null)
                    {
                        context.User = CreatePrincipal(tokenData);
                    }
                }
                catch (Exception ex) when (ex is FormatException or CryptographicException or JsonException)
                {
                    _logger.LogWarning("Invalid authorization token received.");
                    context.Response.Cookies.Delete(LoginController.AuthTokenCookieName);
                }
            }

            await _next(context);
        }

        private static ClaimsPrincipal CreateAnonymousPrincipal()
        {
            // No authentication type, so IsAuthenticated remains false.
            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, AnonymousUserName)],
                authenticationType: null,
                ClaimTypes.Name,
                ClaimTypes.Role);

            return new ClaimsPrincipal(identity);
        }

        private static ClaimsPrincipal CreatePrincipal(AuthTokenData tokenData)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, tokenData.Id),
                new(ClaimTypes.Name, tokenData.Name),
                new(ClaimTypes.Email, tokenData.Email)
            };

            if (tokenData.Claims is not null)
            {
                claims.AddRange(tokenData.Claims
                    .Where(claim => !string.IsNullOrEmpty(claim.Type))
                    .Select(claim => new Claim(claim.Type, claim.Value ?? string.Empty)));
            }

            var identity = new ClaimsIdentity(claims, AuthenticationType, ClaimTypes.Name, ClaimTypes.Role);
            return new ClaimsPrincipal(identity);
        }

        private sealed record AuthTokenData(string Id, string Name, string Email, List<AuthTokenClaim>? Claims);

        private sealed record AuthTokenClaim(string Type, string? Value);
    }
}
