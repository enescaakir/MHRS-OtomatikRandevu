using System.Text.Json.Serialization;

namespace MHRS_OtomatikRandevu.Models.ResponseModels
{
    public class ApiResponse<T> where T : class
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("infos")]
        public object[] Infos { get; set; }

        [JsonPropertyName("warnings")]
        public object[] Warnings { get; set; }

        [JsonPropertyName("wrrors")]
        public object[] Errors { get; set; }
    }
}
