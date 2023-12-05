using System.Text.Json.Serialization;

namespace MHRS_OtomatikRandevu.Models.RequestModels
{
    public class LoginRequestModel
    {
        [JsonPropertyName("kullaniciAdi")]
        public string KullaniciAdi { get; set; }

        [JsonPropertyName("parola")]
        public string Parola { get; set; }

        [JsonPropertyName("islemKanali")]
        public string IslemKanali { get; } = "VATANDAS_WEB";

        [JsonPropertyName("girisTipi")]
        public string GirisTipi { get; } = "PAROLA";
    }
}
