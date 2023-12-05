using System.Net.Http.Headers;

namespace MHRS_OtomatikRandevu.Extensions
{
    public static class HttpRequestHeaderExtension
    {
        public static HttpRequestHeaders AddAuthorization(this HttpRequestHeaders header, string jwtToken)
        {
            header.Add("Authorization", $"Bearer {jwtToken}");
            return header;
        }
    }
}