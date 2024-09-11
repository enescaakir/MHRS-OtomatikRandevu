using System.Text.Json.Serialization;

namespace MHRS_OtomatikRandevu.Models.RequestModels
{
    public class SlotRequestModel
    {
        [JsonPropertyName("aksiyonId")]
        public int AksiyonId { get; } = 200;

        [JsonPropertyName("baslangicZamani")]
        public string BaslangicZamani { get; set; } = "2024-09-11 13:54:32";

        [JsonPropertyName("bitisZamani")]
        public string BitisZamani { get; set; } = "2024-09-13 13:54:32";

        [JsonPropertyName("cinsiyet")]
        public string Cinsiyet { get; } = "F";

        [JsonPropertyName("ekRandevu")]
        public bool EkRandevu { get; } = true;

        [JsonPropertyName("mhrsHekimId")]
        public long MhrsHekimId { get; set; }

        [JsonPropertyName("mhrsIlId")]
        public long MhrsIlId { get; set; }

        [JsonPropertyName("mhrsIlceId")]
        public long MhrsIlceId { get; set; }

        [JsonPropertyName("mhrsKlinikId")]
        public long MhrsKlinikId { get; set; }

        [JsonPropertyName("mhrsKurumId")]
        public long MhrsKurumId { get; set; }

        [JsonPropertyName("muayeneYeriId")]
        public long MuayeneYeriId { get; set; }

        [JsonPropertyName("randevuZamaniList")]
        public List<string> RandevuZamaniList { get; } = new();

        [JsonPropertyName("tumRandevular")]
        public bool TumRandevular { get; } = false;
    }
}
