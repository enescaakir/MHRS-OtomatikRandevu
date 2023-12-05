using MHRS_OtomatikRandevu.Models.ResponseModels;

namespace MHRS_OtomatikRandevu.Services.Abstracts
{
    public interface IClientService
    {
        public ApiResponse<T> Get<T>(string baseUrl, string endpoint) where T : class;

        public T GetSimple<T>(string baseUrl, string endpoint) where T : class;

        public Task<ApiResponse<T>> Post<T>(string baseUrl, string endpoint, object requestModel) where T : class;

        public HttpResponseMessage PostSimple(string baseUrl, string endpoint, object requestModel);

        void AddOrUpdateAuthorizationHeader(string jwtToken);
    }
}
