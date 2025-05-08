using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.Shared.Auth
{
    public interface ITokenStorage 
    {
        Task<string?> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task RemoveTokenAsync();
    }
}
