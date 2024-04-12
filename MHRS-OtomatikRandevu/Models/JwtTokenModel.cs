using System.Text.Json.Serialization;

namespace MHRS_OtomatikRandevu.Models
{
    public class JwtTokenModel
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }

        [JsonPropertyName("exp")]
        public long ExpirationUnix { get; set; }
    }
}
