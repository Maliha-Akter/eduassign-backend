using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache; // ADDED: Memory cache instance

        public BetterAuthHandler(
            IOptionsMonitor<BetterAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache) : base(options, logger, encoder) // ADDED: Injected IMemoryCache
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
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

            // 2. Fallback to Cookies (Checking dev, production Secure, and chunked cookies)
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

            // ===== DEBUG LOG =====
            Console.WriteLine("======================================");
            Console.WriteLine($"BetterAuth token received: {token}");
            Console.WriteLine("======================================");

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.NoResult();
            }

            BetterAuthSessionResponse? sessionData = null;
            string cacheKey = $"betterauth_session_{token}";

            // ===== ADDED: CHECK IN-MEMORY CACHE FIRST =====
            if (_cache.TryGetValue(cacheKey, out BetterAuthSessionResponse? cachedSession) && cachedSession != null)
            {
                sessionData = cachedSession;
            }
            else
            {
                // 3. Validate Token against Next.js Better Auth endpoint
                var client = _httpClientFactory.CreateClient();
                var baseUrl = Options.AuthServerUrl.TrimEnd('/');
                
                var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/auth/get-session");

                // ADDED: Send Origin and User-Agent so Next.js CSRF check doesn't reject server-to-server calls
                request.Headers.Add("Origin", baseUrl);
                request.Headers.Add("User-Agent", "EduAssign-Backend/1.0");

                // Send as Bearer header (Better Auth natively supports this)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                // Also send as Cookie for redundancy depending on Better Auth config
                request.Headers.Add("Cookie", $"better-auth.session_token={token}; __Secure-better-auth.session_token={token}");

                // Disable caching on HTTP client request level
                request.Headers.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MustRevalidate = true
                };

                try
                {
                    var response = await client.SendAsync(request);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        
                        // ===== LOG RAW JSON =====
                        Console.WriteLine($"RAW NEXT.JS RESPONSE: {json}");
                        
                        sessionData = JsonSerializer.Deserialize<BetterAuthSessionResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        // Store valid response in cache for 2 minutes
                        if (sessionData?.Session != null && sessionData?.User != null)
                        {
                            _cache.Set(cacheKey, sessionData, TimeSpan.FromMinutes(2));
                        }
                    }
                    else
                    {
                        return AuthenticateResult.Fail("Invalid Better Auth session.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Auth Server contact failed: {ex.Message}");
                }

                // ===== JWT FALLBACK =====
                if ((sessionData?.User == null || sessionData?.Session == null) && token.Contains('.'))
                {
                    try
                    {
                        var parts = token.Split('.');
                        if (parts.Length == 3)
                        {
                            var payload = parts[1].Replace('-', '+').Replace('_', '/');
                            switch (payload.Length % 4)
                            {
                                case 2: payload += "=="; break;
                                case 3: payload += "="; break;
                            }
                            var decodedJson = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                            
                            Console.WriteLine($"JWT DIRECT DECODE SUCCESS: {decodedJson}");
                            
                            var jwtUser = JsonSerializer.Deserialize<BetterAuthUser>(decodedJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            
                            if (jwtUser != null && !string.IsNullOrEmpty(jwtUser.Id))
                            {
                                sessionData = new BetterAuthSessionResponse 
                                { 
                                    User = jwtUser, 
                                    Session = new BetterAuthSession { Id = "jwt-session" } 
                                };

                                // Cache JWT decoded result for 2 minutes
                                _cache.Set(cacheKey, sessionData, TimeSpan.FromMinutes(2));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"JWT Fallback Error: {ex.Message}");
                    }
                }
            }

            // ===== DEBUG LOG =====
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

            // ADDED: Explicitly specify NameType and RoleType so [Authorize(Roles = "...")] resolves correctly
            var identity = new ClaimsIdentity(claims, Scheme.Name, ClaimTypes.Name, ClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
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
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        // ADDED: Fallback for MongoDB direct _id field
        [JsonPropertyName("_id")]
        public string MongoId 
        { 
            set { if (string.IsNullOrEmpty(Id)) Id = value; } 
        }

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string? Role { get; set; }
    }
}