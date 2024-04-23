using MHRS_OtomatikRandevu.Models;
using MHRS_OtomatikRandevu.Models.RequestModels;
using MHRS_OtomatikRandevu.Models.ResponseModels;
using MHRS_OtomatikRandevu.Services;
using MHRS_OtomatikRandevu.Services.Abstracts;
using MHRS_OtomatikRandevu.Urls;
using MHRS_OtomatikRandevu.Utils;
using System.Net;

namespace MHRS_OtomatikRandevu
{
    public class Program
    {
        static string TC_NO;
        static string SIFRE;

        const string TOKEN_FILE_NAME = "token.txt";
        static string JWT_TOKEN;
        static DateTime TOKEN_END_DATE;

        static IClientService _client;
        static INotificationService _notificationService;

        static void Main(string[] args)
        {
            _client = new ClientService();
            _notificationService = new NotificationService();

            #region Giriş Yap Bölümü
            do
            {
                Console.Clear();
                Console.WriteLine("MHRS Otomatik Randevu Sistemine Hoşgeldiniz.\nLütfen giriş yapmak için bilgilerinizi giriniz.");

                Console.Write("TC: ");
                TC_NO = Console.ReadLine();

                Console.Write("Şifre: ");
                SIFRE = Console.ReadLine();

                Console.WriteLine("Giriş Yapılıyor...");

                var tokenData = GetToken(_client);
                if (string.IsNullOrEmpty(tokenData.Token))
                    continue;

                JWT_TOKEN = tokenData.Token;
                TOKEN_END_DATE = tokenData.Expiration;

                _client.AddOrUpdateAuthorizationHeader(JWT_TOKEN);

            } while (string.IsNullOrEmpty(JWT_TOKEN));
            #endregion

            #region İl Seçim Bölümü
            int provinceIndex;
            var provinceListResponse = _client.GetSimple<List<GenericResponseModel>>(MHRSUrls.BaseUrl, MHRSUrls.GetProvinces);
            if (provinceListResponse == null || !provinceListResponse.Any())
            {
                ConsoleUtil.WriteText("Bir hata meydana geldi!", 2000);
                return;
            }
            var provinceList = provinceListResponse
                                    .DistinctBy(x => x.Value)
                                    .OrderBy(x => x.Value)
                                    .ToList();
            do
            {
                Console.Clear();
                Console.WriteLine("-------------------------------------------");
                for (int i = 0; i < provinceList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}-{provinceList[i].Text}");
                }
                Console.WriteLine("-------------------------------------------");
                Console.Write("İl Numarası (Plaka) Giriniz: ");
                provinceIndex = Convert.ToInt32(Console.ReadLine());

                if (provinceIndex == 34)
                {
                    int subLocationIndex;
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("-------------------------------------------");
                        Console.WriteLine($"0-İSTANBUL\n1-İSTANBUL (AVRUPA)\n2-İSTANBUL (ANADOLU)");
                        Console.WriteLine("-------------------------------------------");

                        Console.Write(@"Alt Bölge Numarası Giriniz: ");
                        subLocationIndex = Convert.ToInt32(Console.ReadLine()); ;
                    } while (subLocationIndex < 0 && subLocationIndex > 2);

                    if (subLocationIndex != 0)
                        provinceIndex = int.Parse("34" + subLocationIndex);
                }

            } while (provinceIndex < 1 || provinceIndex > 81);
            provinceIndex = provinceList[provinceIndex - 1].Value;

            #endregion

            #region İlçe Seçim Bölümü
            int districtIndex;
            var districtList = _client.GetSimple<List<GenericResponseModel>>(MHRSUrls.BaseUrl, string.Format(MHRSUrls.GetDistricts, provinceIndex));
            if (districtList == null || !districtList.Any())
            {
                ConsoleUtil.WriteText("Bir hata meydana geldi!", 2000);
                return;
            }

            do
            {
                Console.Clear();
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("0-FARKETMEZ");
                for (int i = 0; i < districtList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}-{districtList[i].Text}");
                }
                Console.WriteLine("-------------------------------------------");
                Console.Write("İlçe Numarası Giriniz: ");
                districtIndex = Convert.ToInt32(Console.ReadLine()); ;

            } while (districtIndex < 0 || districtIndex > districtList.Count);

            if (districtIndex != 0)
                districtIndex = districtList[districtIndex - 1].Value;
            else
                districtIndex = -1;
            #endregion

            #region Klinik Seçim Bölümü
            int clinicIndex;
            var clinicListResponse = _client.Get<List<GenericResponseModel>>(MHRSUrls.BaseUrl, string.Format(MHRSUrls.GetClinics, provinceIndex, districtIndex));
            if (!clinicListResponse.Success && (clinicListResponse.Data == null || !clinicListResponse.Data.Any()))
            {
                ConsoleUtil.WriteText("Bir hata meydana geldi!", 2000);
                return;
            }
            var clinicList = clinicListResponse.Data;
            do
            {
                Console.Clear();
                Console.WriteLine("-------------------------------------------");
                for (int i = 0; i < clinicList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}-{clinicList[i].Text}");
                }
                Console.WriteLine("-------------------------------------------");
                Console.Write("Klinik Numarası Giriniz: ");
                clinicIndex = Convert.ToInt32(Console.ReadLine()); ;

            } while (clinicIndex < 1 || clinicIndex > clinicList.Count);
            clinicIndex = clinicList[clinicIndex - 1].Value;
            #endregion

            #region Hastane Seçim Bölümü
            int hospitalIndex;
            var hospitalListResponse = _client.Get<List<GenericResponseModel>>(MHRSUrls.BaseUrl, string.Format(MHRSUrls.GetHospitals, provinceIndex, districtIndex, clinicIndex));
            if (!hospitalListResponse.Success && (hospitalListResponse.Data == null || !hospitalListResponse.Data.Any()))
            {
                ConsoleUtil.WriteText("Bir hata meydana geldi!", 2000);
                return;
            }
            var hospitalList = hospitalListResponse.Data;
            do
            {
                Console.Clear();
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("0-FARKETMEZ");
                for (int i = 0; i < hospitalList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}-{hospitalList[i].Text}");
                }
                Console.WriteLine("-------------------------------------------");
                Console.Write("Hastane Numarası Giriniz: ");
                hospitalIndex = Convert.ToInt32(Console.ReadLine()); ;
            } while (hospitalIndex < 0 || hospitalIndex > hospitalList.Count);

            if (hospitalIndex != 0)
            {
                var hospital = hospitalList[hospitalIndex - 1];
                if (hospital.Children.Any())
                {
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("-------------------------------------------");
                        Console.WriteLine($"0-{hospital.Text}");
                        for (int i = 0; i < hospital.Children.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}-{hospital.Children[i].Text}");
                        }
                        Console.WriteLine("-------------------------------------------");
                        Console.Write("Hastane/Poliklinik Numarası Giriniz: ");
                        hospitalIndex = Convert.ToInt32(Console.ReadLine()); ;
                    } while (0 < hospitalIndex || hospitalIndex > hospital.Children.Count);

                    if (hospitalIndex == 0)
                        hospitalIndex = hospital.Value;
                    else
                        hospitalIndex = hospital.Children[hospitalIndex - 1].Value;
                }
                else
                {
                    hospitalIndex = hospitalList[hospitalIndex - 1].Value;
                }

            }
            else
            {
                hospitalIndex = -1;
            }

            #endregion

            #region Muayene Yeri Seçim Bölümü
            int placeIndex;
            var placeListResponse = _client.Get<List<ClinicResponseModel>>(MHRSUrls.BaseUrl, string.Format(MHRSUrls.GetPlaces, hospitalIndex, clinicIndex));
            if (!placeListResponse.Success && (placeListResponse.Data == null || !placeListResponse.Data.Any()))
            {
                ConsoleUtil.WriteText("Bir hata meydana geldi!", 2000);
                return;
            }
            var placeList = placeListResponse.Data;

            do
            {
                Console.Clear();
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("0-FARKETMEZ");
                for (int i = 0; i < placeList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}-{placeList[i].Text}");
                }
                Console.WriteLine("-------------------------------------------");
                Console.Write("Muayene Yeri Numarası Giriniz: ");
                placeIndex = Convert.ToInt32(Console.ReadLine()); ;
            } while (placeIndex < 0 || placeIndex > placeList.Count);

            if (placeIndex != 0)
                placeIndex = placeList[placeIndex - 1].Value;
            else
                placeIndex = -1;

            #endregion

            #region Doktor Seçim Bölümü
            int doctorIndex;
            var doctorListResponse = _client.Get<List<GenericResponseModel>>(MHRSUrls.BaseUrl, string.Format(MHRSUrls.GetDoctors, hospitalIndex, clinicIndex));
            if (!doctorListResponse.Success && (doctorListResponse.Data == null || !doctorListResponse.Data.Any()))
            {
                ConsoleUtil.WriteText("Bir hata meydana geldi!", 2000);
                return;
            }
            var doctorList = doctorListResponse.Data;
            do
            {
                Console.Clear();
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("0-FARKETMEZ");
                for (int i = 0; i < doctorList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}-{doctorList[i].Text}");
                }
                Console.WriteLine("-------------------------------------------");
                Console.Write("Doktor Numarası Giriniz: ");
                doctorIndex = Convert.ToInt32(Console.ReadLine()); ;
            } while (doctorIndex < 0 || doctorIndex > doctorList.Count);

            if (doctorIndex != 0)
                doctorIndex = doctorList[doctorIndex - 1].Value;
            else
                doctorIndex = -1;

            Console.Clear();
            #endregion

            #region Randevu Alım Bölümü
            bool sendNotification = false;

            Console.WriteLine("SMS ile bildirim almak ister misiniz? (e) Evet / (h) Hayır");
            string sendNotificationAnswer = Console.ReadLine() ?? "h";

            if (sendNotificationAnswer is "e" or "E")
                sendNotification = true;

            ConsoleUtil.WriteText("Yapmış olduğunuz seçimler doğrultusunda müsait olan ilk randevu otomatik olarak alınacaktır.\nEğer SMS bildirimini onayladıysanız randevu tarihi SMS olarak iletilecektir.", 3000);
            Console.Clear();

            bool appointmentState = false;
            bool isNotified = false;
            do
            {
                if (TOKEN_END_DATE == default || TOKEN_END_DATE < DateTime.Now)
                {
                    var tokenData = GetToken(_client);
                    if (string.IsNullOrEmpty(tokenData.Token))
                    {
                        ConsoleUtil.WriteText("Yeniden giriş yapılırken bir hata meydana geldi!", 2000);
                        return;
                    }
                    JWT_TOKEN = tokenData.Token;
                    _client.AddOrUpdateAuthorizationHeader(JWT_TOKEN);
                }

                var slotRequestModel = new SlotRequestModel
                {
                    MhrsHekimId = doctorIndex,
                    MhrsIlId = provinceIndex,
                    MhrsIlceId = districtIndex,
                    MhrsKlinikId = clinicIndex,
                    MhrsKurumId = hospitalIndex,
                    MuayeneYeriId = placeIndex
                };

                var slot = GetSlot(_client, slotRequestModel);
                if (slot == null || slot == default)
                {
                    Console.WriteLine($"Müsait randevu bulunamadı | Kontrol Saati: {DateTime.Now.ToShortTimeString()}");
                    Thread.Sleep(TimeSpan.FromMinutes(5));
                    continue;
                }

                var appointmentRequestModel = new AppointmentRequestModel
                {
                    FkSlotId = slot.Id,
                    FkCetvelId = slot.FkCetvelId,
                    MuayeneYeriId = slot.MuayeneYeriId,
                    BaslangicZamani = slot.BaslangicZamani,
                    BitisZamani = slot.BitisZamani
                };

                Console.WriteLine($"Randevu bulundu - Müsait Tarih: {slot.BaslangicZamani}");
                if (!isNotified && sendNotification)
                    _notificationService.SendNotification($"\n\nRandevu bulundu - Müsait Tarih: {slot.BaslangicZamani}").Wait();

                appointmentState = MakeAppointment(_client, appointmentRequestModel, sendNotification);
            } while (!appointmentState);
            #endregion

            Console.ReadKey();
        }

        static JwtTokenModel GetToken(IClientService client)
        {
            var rawPath = Directory.GetCurrentDirectory()
                .Split("\\bin\\")
                .SkipLast(1)
                .FirstOrDefault();
            var tokenFilePath = Path.Combine(rawPath, TOKEN_FILE_NAME);
            try
            {

                var tokenData = File.ReadAllText(tokenFilePath);
                if (string.IsNullOrEmpty(tokenData) || JwtTokenUtil.GetTokenExpireTime(tokenData) < DateTime.Now)
                    throw new Exception();

                return new() { Token = tokenData, Expiration = JwtTokenUtil.GetTokenExpireTime(tokenData) };
            }
            catch (Exception)
            {
                var loginRequestModel = new LoginRequestModel
                {
                    KullaniciAdi = TC_NO,
                    Parola = SIFRE
                };

                var loginResponse = client.Post<LoginResponseModel>(MHRSUrls.BaseUrl, MHRSUrls.Login, loginRequestModel).Result;
                if (loginResponse.Data == null || (loginResponse.Data != null && string.IsNullOrEmpty(loginResponse.Data?.Jwt)))
                {
                    ConsoleUtil.WriteText("Giriş yapılırken bir hata meydana geldi!", 2000);
                    return null;
                }

                File.WriteAllText(tokenFilePath, loginResponse.Data!.Jwt);

                return new() { Token = loginResponse.Data!.Jwt, Expiration = JwtTokenUtil.GetTokenExpireTime(loginResponse.Data!.Jwt) };
            }
        }

        //Aynı gün içerisinde tek slot mevcut ise o slotu bulur
        //Aynı gün içerisinde birden fazla slot mevcut ise en yakın saati getirmez fakat en yakın güne ait bir slot getirir
        static SubSlot GetSlot(IClientService client, SlotRequestModel slotRequestModel)
        {
            var slotListResponse = client.Post<List<SlotResponseModel>>(MHRSUrls.BaseUrl, MHRSUrls.GetSlots, slotRequestModel).Result;
            if (slotListResponse.Data is null)
            {
                ConsoleUtil.WriteText("Bir hata meydana geldi!", 2000);
                return null;
            }

            var saatSlotList = slotListResponse.Data.FirstOrDefault()?.HekimSlotList.FirstOrDefault()?.MuayeneYeriSlotList.FirstOrDefault()?.SaatSlotList;
            if (saatSlotList == null || !saatSlotList.Any())
                return null;

            var slot = saatSlotList.FirstOrDefault(x => x.Bos)?.SlotList.FirstOrDefault(x => x.Bos)?.SubSlot;
            if (slot == default)
                return null;

            return slot;
        }

        static bool MakeAppointment(IClientService client, AppointmentRequestModel appointmentRequestModel, bool sendNotification)
        {
            var randevuResp = client.PostSimple(MHRSUrls.BaseUrl, MHRSUrls.MakeAppointment, appointmentRequestModel);
            if (randevuResp.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Randevu alırken bir problem ile karşılaşıldı! \nRandevu Tarihi -> {appointmentRequestModel.BaslangicZamani}");
                return false;
            }

            var message = $"Randevu alındı! \nRandevu Tarihi -> {appointmentRequestModel.BaslangicZamani}";
            Console.WriteLine(message);

            if (sendNotification)
                _notificationService.SendNotification(message).Wait();

            return true;
        }
    }
}
