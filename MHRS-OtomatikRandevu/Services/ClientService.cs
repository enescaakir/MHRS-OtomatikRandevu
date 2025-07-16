using MHRS_OtomatikRandevu.Extensions;
using MHRS_OtomatikRandevu.Models.ResponseModels;
using MHRS_OtomatikRandevu.Services.Abstracts;
using System.Net.Http.Json;
using System.Text.Json;

namespace MHRS_OtomatikRandevu.Services
{
    public class ClientService : IClientService
    {
        private HttpClient _client;

        public ClientService()
        {
            _client = new HttpClient();
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string baseUrl, string endpoint) where T : class
        {
            var response = await _client.GetFromJsonAsync<ApiResponse<T>>(string.Concat(baseUrl, endpoint));
            if (response is null || (response.Warnings != null && response.Warnings.Any()) || (response.Errors != null && response.Errors.Any()))
                return new();

            return response;
        }

        public async Task<T> GetSimpleAsync<T>(string baseUrl, string endpoint) where T : class
        {
            var response = await _client.GetFromJsonAsync<T>(string.Concat(baseUrl, endpoint));
            return response;
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string baseUrl, string endpoint, object requestModel) where T : class
        {
            var response = await _client.PostAsJsonAsync(string.Concat(baseUrl, endpoint), requestModel);
            var data = response.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(data))
                return new();

            var mappedData = JsonSerializer.Deserialize<ApiResponse<T>>(data);
            return mappedData ?? new();
        }

        public async Task<HttpResponseMessage> PostSimpleAsync(string baseUrl, string endpoint, object requestModel)
        {
            return await _client.PostAsJsonAsync(string.Concat(baseUrl, endpoint), requestModel);
        }

        public void AddOrUpdateAuthorizationHeader(string jwtToken)
        {
            if (_client.DefaultRequestHeaders.Any(x => x.Key == "Authorization"))
                _client.DefaultRequestHeaders.Remove("Authorization");

            _client.DefaultRequestHeaders.AddAuthorization(jwtToken);
        }
    }
}