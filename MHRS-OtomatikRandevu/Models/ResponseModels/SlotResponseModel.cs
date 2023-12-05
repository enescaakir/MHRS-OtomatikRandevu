using System.Text.Json.Serialization;

namespace MHRS_OtomatikRandevu.Models.ResponseModels
{
    public class SlotResponseModel
    {
        [JsonPropertyName("hekimSlotList")]
        public List<DoctorSlot> HekimSlotList { get; set; }
    }

    public class DoctorSlot
    {
        [JsonPropertyName("muayeneYeriSlotList")]
        public List<PlaceSlot> MuayeneYeriSlotList { get; set; }
    }

    public class PlaceSlot
    {
        [JsonPropertyName("saatSlotList")]
        public List<DateSlot> SaatSlotList { get; set; }
    }

    public class DateSlot
    {
        [JsonPropertyName("slotList")]
        public List<Slot> SlotList { get; set; }

        [JsonPropertyName("bos")]
        public bool Bos { get; set; }
    }

    public class Slot
    {
        [JsonPropertyName("slot")]
        public SubSlot SubSlot { get; set; }

        [JsonPropertyName("bos")]
        public bool Bos { get; set; }
    }

    public class SubSlot
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("fkCetvelId")]
        public long FkCetvelId { get; set; }

        [JsonPropertyName("muayeneYeriId")]
        public long MuayeneYeriId { get; set; }

        [JsonPropertyName("baslangicZamani")]
        public string BaslangicZamani { get; set; }

        [JsonPropertyName("bitisZamani")]
        public string BitisZamani { get; set; }

        [JsonPropertyName("kalanKullanim")]
        public int KalanKullanim { get; set; }
    }
}
