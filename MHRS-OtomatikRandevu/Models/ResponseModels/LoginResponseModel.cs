using System.Text.Json.Serialization;

namespace MHRS_OtomatikRandevu.Models.ResponseModels
{
    public class LoginResponseModel
    {
        [JsonPropertyName("jwt")]
        public string Jwt { get; set; }
    }
}