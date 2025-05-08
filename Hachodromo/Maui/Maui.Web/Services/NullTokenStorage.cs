using Maui.Shared.Auth;

namespace Maui.Web.Services
{
    public class NullTokenStorage : ITokenStorage
    {
        public Task<string?> GetTokenAsync() => Task.FromResult<string?>(null);
        public Task SetTokenAsync(string token) => Task.CompletedTask;
        public Task RemoveTokenAsync() => Task.CompletedTask;
    }
}
