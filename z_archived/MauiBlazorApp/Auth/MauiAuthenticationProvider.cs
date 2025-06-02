using Hachodromo.Shared.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Hachodromo.MAUI.Auth;

/// <summary>Proveedor de autenticación para MAUI (Blazor Hybrid).  
/// Usa SecureStorage en lugar de localStorage.</summary>
public sealed class MauiAuthenticationProvider : AuthenticationStateProvider, ILoginService
{
    private readonly HttpClient _http;
    private const string TokenKey = "token";
    private readonly AuthenticationState _anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public MauiAuthenticationProvider(HttpClient http) => _http = http;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await SecureStorage.Default.GetAsync(TokenKey);
        return string.IsNullOrEmpty(token) ? _anonymous : BuildState(token);
    }

    public async Task LoginAsync(string token)
    {
        await SecureStorage.Default.SetAsync(TokenKey, token);
        Notify(BuildState(token));
    }

    public Task LogoutAsync()
    {
        SecureStorage.Default.Remove(TokenKey);
        _http.DefaultRequestHeaders.Authorization = null;
        Notify(_anonymous);
        return Task.CompletedTask;
    }

    /* ---------- helpers ---------- */

    private AuthenticationState BuildState(string token)
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
    }

    private void Notify(AuthenticationState state) =>
        NotifyAuthenticationStateChanged(Task.FromResult(state));
}
