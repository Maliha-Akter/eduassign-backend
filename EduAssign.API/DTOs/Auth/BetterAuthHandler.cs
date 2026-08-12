using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace EduAssign.API.DTOs.Auth
{
    public class BetterAuthOptions : AuthenticationSchemeOptions
    {
        // Default to localhost, but easily configurable in Program.cs
        public string AuthServerUrl { get; set; } = "http://localhost:3000";
    }

    public class BetterAuthHandler : AuthenticationHandler<BetterAuthOptions>
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BetterAuthHandler(
            IOptionsMonitor<BetterAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IHttpClientFactory httpClientFactory) : base(options, logger, encoder)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? token = null;

            // 1. Try to get token from Authorization header
            if (Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var headerValue = authHeader.ToString();
                if (headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = headerValue.Substring("Bearer ".Length).Trim();
                }
            }

            // 2. Fallback to Cookies (Checking both dev and production Secure cookies)
            if (string.IsNullOrEmpty(token))
            {
                if (Request.Cookies.TryGetValue("better-auth.session_token", out var devCookie))
                {
                    token = devCookie;
                }
                else if (Request.Cookies.TryGetValue("__Secure-better-auth.session_token", out var prodCookie))
                {
                    token = prodCookie;
                }
            }

            // ===== ADDED DEBUG LOG =====
            Console.WriteLine("======================================");
            Console.WriteLine($"BetterAuth token received: {token}");
            Console.WriteLine("======================================");

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.NoResult();
            }
            // 3. Validate Token against Next.js Better Auth endpoint
            var client = _httpClientFactory.CreateClient();
            var baseUrl = Options.AuthServerUrl.TrimEnd('/');
            
            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/auth/get-session");

            // Send as Bearer header (Better Auth natively supports this)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            // Also send as Cookie for redundancy depending on Better Auth config
            request.Headers.Add("Cookie", $"better-auth.session_token={token}; __Secure-better-auth.session_token={token}");

            // Disable caching
            request.Headers.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true,
                MustRevalidate = true
            };

            try
            {
                var response = await client.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    return AuthenticateResult.Fail("Invalid Better Auth session.");
                }

                var json = await response.Content.ReadAsStringAsync();
                var sessionData = JsonSerializer.Deserialize<BetterAuthSessionResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // ===== ADDED DEBUG LOG =====
                Console.WriteLine("======================================");
                Console.WriteLine($"BetterAuth User ID: {sessionData?.User?.Id}");
                Console.WriteLine($"BetterAuth User Name: {sessionData?.User?.Name}");
                Console.WriteLine($"BetterAuth User Email: {sessionData?.User?.Email}");
                Console.WriteLine($"BetterAuth User Role: {sessionData?.User?.Role}");
                Console.WriteLine("======================================");

                if (sessionData?.Session == null || sessionData.User == null)
                {
                    return AuthenticateResult.Fail("Session expired or invalid format.");
                }

                var role = sessionData.User.Role?.ToLowerInvariant() ?? "student";

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, sessionData.User.Id),
                    new Claim(ClaimTypes.Email, sessionData.User.Email ?? ""),
                    new Claim(ClaimTypes.Name, sessionData.User.Name ?? ""),
                    new Claim(ClaimTypes.Role, role) 
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Failed to contact Auth Server: {ex.Message}");
            }
        }
    }

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