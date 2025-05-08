using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Maui.Shared.Auth
{
    public class AuthenticationProviderTest : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //  await Task.Delay(3000); // Simulate a delay for testing purposes
            var anonimouis = new ClaimsIdentity();
            var joseUser = new ClaimsIdentity(new List<Claim>
            {
                new Claim("FirstName", "Jose"),
                new Claim("LastName", "Flores"),
                new Claim(ClaimTypes.Name, "euroflores1@gmail.com"),
                new Claim(ClaimTypes.Role, "Admin"),
            },
            authenticationType: "test");
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonimouis)));
        }
    }
}
