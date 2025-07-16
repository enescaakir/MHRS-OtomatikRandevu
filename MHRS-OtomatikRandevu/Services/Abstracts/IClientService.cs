using MHRS_OtomatikRandevu.Models.ResponseModels;

namespace MHRS_OtomatikRandevu.Services.Abstracts
{
    public interface IClientService
    {
        public Task<ApiResponse<T>> GetAsync<T>(string baseUrl, string endpoint) where T : class;

        public Task<T> GetSimpleAsync<T>(string baseUrl, string endpoint) where T : class;

        public Task<ApiResponse<T>> PostAsync<T>(string baseUrl, string endpoint, object requestModel) where T : class;

        public Task<HttpResponseMessage> PostSimpleAsync(string baseUrl, string endpoint, object requestModel);

        void AddOrUpdateAuthorizationHeader(string jwtToken);
    }
}
