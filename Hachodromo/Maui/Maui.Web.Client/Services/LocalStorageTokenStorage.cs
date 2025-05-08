using Maui.Shared.Auth;
using Microsoft.JSInterop;

namespace Maui.Web.Client.Services
{
    public class LocalStorageTokenStorage : ITokenStorage
    {
        private readonly IJSRuntime _js;
        private const string TokenKey = "token";

        public LocalStorageTokenStorage(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<string?> GetTokenAsync()
            => await _js.InvokeAsync<string>("localStorage.getItem", TokenKey);

        public async Task SetTokenAsync(string token)
            => await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);

        public async Task RemoveTokenAsync()
            => await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
    }
}

