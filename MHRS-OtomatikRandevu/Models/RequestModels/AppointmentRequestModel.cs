using System.Text.Json.Serialization;

namespace MHRS_OtomatikRandevu.Models.RequestModels
{
    public class AppointmentRequestModel
    {
        [JsonPropertyName("fkSlotId")]
        public long FkSlotId { get; set; }

        [JsonPropertyName("fkCetvelId")]
        public long FkCetvelId { get; set; }

        [JsonPropertyName("yenidogan")]
        public bool Yenidogan { get; } = false;

        [JsonPropertyName("muayeneYeriId")]
        public long MuayeneYeriId { get; set; }

        [JsonPropertyName("baslangicZamani")]
        public string BaslangicZamani { get; set; }

        [JsonPropertyName("bitisZamani")]
        public string BitisZamani { get; set; }

        [JsonPropertyName("randevuNotu")]
        public string RandevuNotu { get;} = "";
    }
}
