using Hachodromo.Shared.Responses;

namespace Hachodromo.API.Services
{
    public interface IApiService
    {
        Task<Response> GetListAsync<T>(string servicePrefix, string controller);
    }
}
