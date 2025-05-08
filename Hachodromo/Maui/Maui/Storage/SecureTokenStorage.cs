using Maui.Shared.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.Storage
{
    public class SecureTokenStorage : ITokenStorage
    {
        private const string TokenKey = "token";

        public async Task<string?> GetTokenAsync()
            => await SecureStorage.Default.GetAsync(TokenKey);

        public async Task SetTokenAsync(string token)
            => await SecureStorage.Default.SetAsync(TokenKey, token);

        public Task RemoveTokenAsync()
        {
            SecureStorage.Default.Remove(TokenKey);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            throw new NotImplementedException();

        }
    }
}
