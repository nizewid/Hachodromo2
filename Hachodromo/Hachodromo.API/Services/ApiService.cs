﻿using Newtonsoft.Json;
using Hachodromo.Shared.Responses;

namespace Hachodromo.API.Services
{
    public class ApiService : IApiService
    {
        private readonly string _urlBase;
        private readonly string _tokenName;
        private readonly string _tokenValue;

        public ApiService(IConfiguration configuration)
        {
            _urlBase = configuration["CoutriesAPI:urlBase"]!;
            _tokenName = configuration["CoutriesAPI:tokenName"]!;
            _tokenValue = configuration["CoutriesAPI:tokenValue"]!;
        }


        public async Task<Response> GetListAsync<T>(string servicePrefix, string controller)
        {
            try
            {
                HttpClient client = new()
                {
                    BaseAddress = new Uri(_urlBase),
                };

                client.DefaultRequestHeaders.Add(_tokenName, _tokenValue);
                string url = $"{servicePrefix}{controller}";
                HttpResponseMessage response = await client.GetAsync(url);
                string result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                        StatusCode = (int)response.StatusCode,
                        Errors = new List<string> { result },
                    };
                }

                List<T> list = JsonConvert.DeserializeObject<List<T>>(result)!;
                return new Response
                {
                    IsSuccess = true,
                    StatusCode = (int)response.StatusCode,
                    Result = list
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
