using Maui.Shared.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Maui.Shared.Auth
{
    public class AuthenticationProviderJWT : AuthenticationStateProvider, ILoginService
    {
        private readonly ITokenStorage _tokenStorage;
        private readonly HttpClient _httpClient;
        private readonly AuthenticationState _anonymousUser;

        public AuthenticationProviderJWT(ITokenStorage tokenStorage, HttpClient httpClient)
        {
            _tokenStorage = tokenStorage;
            _httpClient = httpClient;
            _anonymousUser = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokenStorage.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                return _anonymousUser;
            }

            return BuildAuthenticationState(token);
        }

        private AuthenticationState BuildAuthenticationState(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var claims = ParseClaimsFromJWT(token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }

        private IEnumerable<Claim>? ParseClaimsFromJWT(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims;
        }

        public async Task LoginAsync(string token)
        {
            await _tokenStorage.SetTokenAsync(token);
            var authState = BuildAuthenticationState(token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task LogoutAsync()
        {
            await _tokenStorage.RemoveTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymousUser));
        }
    }
}
