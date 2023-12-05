using System.Text.Json.Serialization;

namespace MHRS_OtomatikRandevu.Models.ResponseModels
{
    public class DistrictResponseModel
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
