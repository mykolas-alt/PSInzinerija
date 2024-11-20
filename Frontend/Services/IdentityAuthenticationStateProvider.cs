using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;

using PSInzinerija1.Shared.Data.Models;

namespace Frontend.Services
{
    public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public IdentityAuthenticationStateProvider(IHttpClientFactory httpClientFactory, ILogger<IdentityAuthenticationStateProvider> logger)
        {
            _httpClient = httpClientFactory?.CreateClient("BackendApi") ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {

            UserInfo? userInfo = null;
            try
            {
                userInfo = await _httpClient.GetFromJsonAsync<UserInfo>("user/info");
            }
            catch (Exception e)
            {
                _logger.LogInformation("{errorMessage}", e.Message);
            }

            if (userInfo == null)
            {
                return new AuthenticationState(new());
            }

            var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, userInfo.UserName),
                        new(ClaimTypes.Email, userInfo.Email)
                    };

            var identity = new ClaimsIdentity(claims, "Identity");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            return new AuthenticationState(claimsPrincipal);
        }
    }
}
