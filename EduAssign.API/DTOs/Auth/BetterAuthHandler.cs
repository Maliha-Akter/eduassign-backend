using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace EduAssign.API.DTOs.Auth
{
    public class BetterAuthOptions : AuthenticationSchemeOptions { }

    public class BetterAuthHandler : AuthenticationHandler<BetterAuthOptions>
    {
        private readonly HttpClient _httpClient;

        public BetterAuthHandler(
            IOptionsMonitor<BetterAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            HttpClient httpClient) : base(options, logger, encoder)
        {
            _httpClient = httpClient;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? token = null;

            // 1. Try to get the token from the Authorization header (Bearer token)
            if (Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var headerValue = authHeader.ToString();
                if (headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = headerValue.Substring("Bearer ".Length).Trim();
                }
            }

            // 2. Fallback: Try to get it directly from the cookies
            if (string.IsNullOrEmpty(token) && Request.Cookies.TryGetValue("better-auth.session_token", out var cookieToken))
            {
                token = cookieToken;
            }

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.NoResult();
            }

            // 3. Ask Next.js if this Better Auth token is valid
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:3000/api/auth/get-session");
            request.Headers.Add("Cookie", $"better-auth.session_token={token}");

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return AuthenticateResult.Fail("Invalid Better Auth session.");
                }

                var json = await response.Content.ReadAsStringAsync();
                var sessionData = JsonSerializer.Deserialize<BetterAuthSessionResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (sessionData?.Session == null || sessionData.User == null)
                {
                    return AuthenticateResult.Fail("Session expired.");
                }

                // 4. Map Next.js User to ASP.NET Claims
                var rawRole = sessionData.User.Role ?? "Teacher";
var capitalizedRole = char.ToUpper(rawRole[0]) + rawRole.Substring(1).ToLower();

var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, sessionData.User.Id),
    new Claim(ClaimTypes.Email, sessionData.User.Email ?? ""),
    new Claim(ClaimTypes.Name, sessionData.User.Name ?? ""),
    new Claim(ClaimTypes.Role, capitalizedRole) // This will now correctly evaluate as "Teacher"
};

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Failed to contact Next.js: {ex.Message}");
            }
        }
    }

    // Classes to parse the Better Auth JSON response
    public class BetterAuthSessionResponse
    {
        public BetterAuthSession? Session { get; set; }
        public BetterAuthUser? User { get; set; }
    }

    public class BetterAuthSession
    {
        public string Id { get; set; } = string.Empty;
    }

    public class BetterAuthUser
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}