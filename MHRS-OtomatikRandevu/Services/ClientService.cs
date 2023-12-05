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

        public ApiResponse<T> Get<T>(string baseUrl, string endpoint) where T : class
        {
            var response = _client.GetFromJsonAsync<ApiResponse<T>>(string.Concat(baseUrl, endpoint)).Result;
            if ((response.Warnings != null && response.Warnings.Any()) || (response.Errors != null && response.Errors.Any()))
                return new();

            return response;
        }

        public T GetSimple<T>(string baseUrl, string endpoint) where T : class
        {
            return _client.GetFromJsonAsync<T>(string.Concat(baseUrl, endpoint)).Result;
        }

        public async Task<ApiResponse<T>> Post<T>(string baseUrl, string endpoint, object requestModel) where T : class
        {
            var response = await _client.PostAsJsonAsync(string.Concat(baseUrl, endpoint), requestModel);
            var data = response.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(data))
                return new();

            var mappedData = JsonSerializer.Deserialize<ApiResponse<T>>(data);
            return mappedData;
        }

        public HttpResponseMessage PostSimple(string baseUrl, string endpoint, object requestModel)
        {
            return _client.PostAsJsonAsync(string.Concat(baseUrl, endpoint), requestModel).Result;
        }

        public void AddOrUpdateAuthorizationHeader(string jwtToken)
        {
            if (_client.DefaultRequestHeaders.Any(x => x.Key == "Authorization"))
                _client.DefaultRequestHeaders.Remove("Authorization");

            _client.DefaultRequestHeaders.AddAuthorization(jwtToken);
        }
    }
}