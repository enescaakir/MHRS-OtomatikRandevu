namespace MHRS_OtomatikRandevu.Urls
{
    public static class MHRSUrls
    {
        public static string BaseUrl = "https://prd.mhrs.gov.tr";

        public static string Login = "/api/vatandas/login";

        public static string GetProvinces = "/api/yonetim/genel/il/selectinput-tree";
        public static string GetDistricts = "/api/yonetim/genel/ilce/selectinput/{0}";

        public static string GetClinics = "/api/kurum/kurum/kurum-klinik/il/{0}/ilce/{1}/kurum/-1/aksiyon/200/select-input";
        public static string GetHospitals = "/api/kurum/kurum/kurum-klinik/il/{0}/ilce/{1}/kurum/-1/klinik/{2}/ana-kurum/select-input";

        public static string GetPlaces = "/api/kurum/kurum/muayene-yeri/ana-kurum/{0}/kurum/-1/klinik/{1}/select-input";

        public static string GetDoctors = "/api/kurum/hekim/hekim-klinik/hekim-select-input/anakurum/{0}/kurum/-1/klinik/{1}";

        public static string GetSlots = "/api/kurum-rss/randevu/slot-sorgulama/slot";

        public static string MakeAppointment = "/api/kurum/randevu/randevu-ekle";
    }
}
