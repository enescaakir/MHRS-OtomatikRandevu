﻿using MHRS_OtomatikRandevu.Models;
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

        static string JWT_TOKEN;
        static DateTime TOKEN_END_DATE;

        static IClientService _client;

        static async Task Main(string[] args)
        {
            _client = new ClientService();

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

                var tokenData = await GetToken(_client);
                if (tokenData is null || string.IsNullOrEmpty(tokenData.Token))
                    continue;

                JWT_TOKEN = tokenData.Token;
                TOKEN_END_DATE = tokenData.Expiration;

                _client.AddOrUpdateAuthorizationHeader(JWT_TOKEN);

            } while (string.IsNullOrEmpty(JWT_TOKEN));
            #endregion

            #region İl Seçim Bölümü
            int provinceIndex = 0;
            var provinceListResponse = await _client.GetSimpleAsync<List<GenericResponseModel>>(MHRSUrls.BaseUrl, MHRSUrls.GetProvinces);
            if (provinceListResponse is null || !provinceListResponse.Any())
            {
                ConsoleUtil.WriteText("İl listesi alınırken bir hata meydana geldi!", 2000);
                return;
            }
            var provinceList = provinceListResponse
                                     .DistinctBy(x => x.Value)
                                     .OrderBy(x => x.Value)
                                     .ToList();
            var istanbulSubLocationIds = new int[] { 341, 342 };

            do
            {
                Console.Clear();
                Console.WriteLine("-------------------------------------------");
                for (int i = 0; i < provinceList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}-{provinceList[i].Text}");
                }
                Console.WriteLine("-------------------------------------------");
                Console.Write("Randevu almak istediğiniz ilin plaka kodunu giriniz: ");

                try
                {
                    provinceIndex = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    // FIX: FormatException durumunda döngünün tekrar çalışması için geçersiz değer ata.
                    provinceIndex = 0;
                }


                if (provinceIndex is 34)
                {
                    int subLocationIndex = -1;
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("-------------------------------------------");
                        Console.WriteLine($"0-İSTANBUL (Genel)\n1-İSTANBUL (AVRUPA)\n2-İSTANBUL (ANADOLU)");
                        Console.WriteLine("-------------------------------------------");
                        Console.Write(@"Alt Bölge Numarası Giriniz: ");

                        try
                        {
                            subLocationIndex = Convert.ToInt32(Console.ReadLine());
                        }
                        catch (FormatException)
                        {
                            // FIX: FormatException durumunda iç döngünün tekrar çalışması için geçersiz değer ata.
                            subLocationIndex = -1;
                        }
                    } while (subLocationIndex < 0 || subLocationIndex > 2);

                    if (subLocationIndex != 0)
                    {
                        provinceIndex = int.Parse("34" + subLocationIndex);
                    }
                    // subLocationIndex 0 ise provinceIndex 34 kalır
                }

            } while ((provinceIndex < 1 || provinceIndex > 81) && !istanbulSubLocationIds.Contains(provinceIndex));

            Console.WriteLine($"\nSeçilen İl/Bölge Kodu: {provinceIndex}");

            #endregion

            #region İlçe Seçim Bölümü
            int districtIndex = -1;
            var districtList = await _client.GetSimpleAsync<List<GenericResponseModel>>(MHRSUrls.BaseUrl, string.Format(MHRSUrls.GetDistricts, provinceIndex));
            if (districtList is null)
            {
                ConsoleUtil.WriteText($"İlçe listesi alınamadı (İl Kodu: {provinceIndex}). Program sonlandırılıyor.", 3000);
                return;
            }
            if (!districtList.Any())
            {
                ConsoleUtil.WriteText("Seçilen il için ilçe bulunamadı. Program sonlandırılıyor.", 3000);
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
                try
                {
                    districtIndex = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    // FIX: FormatException durumunda döngünün tekrar çalışması için geçersiz değer ata.
                    districtIndex = -1;
                }

            } while (districtIndex < 0 || districtIndex > districtList.Count);

            if (districtIndex != 0)
                districtIndex = districtList[districtIndex - 1].Value;
            else
                districtIndex = -1;
            #endregion

            #region Klinik Seçim Bölümü
            int clinicIndex = 0;
            var clinicListResponse = await _client.GetAsync<List<GenericResponseModel>>(MHRSUrls.BaseUrl, string.Format(MHRSUrls.GetClinics, provinceIndex, districtIndex));
            if (clinicListResponse is null || !clinicListResponse.Success || clinicListResponse.Data is null || !clinicListResponse.Data.Any())
            {
                ConsoleUtil.WriteText("Seçilen il/ilçe için uygun klinik bilgisi alınamadı. Lütfen daha sonra tekrar deneyin.", 3000);
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
                try
                {
                    clinicIndex = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    // FIX: FormatException durumunda döngünün tekrar çalışması için geçersiz değer ata.
                    clinicIndex = 0;
                }

            } while (clinicIndex < 1 || clinicIndex > clinicList.Count);
            clinicIndex = clinicList[clinicIndex - 1].Value;
            #endregion

            #region Hastane Seçim Bölümü
            int hospitalIndex = -1;
            var hospitalListResponse = await _client.GetAsync<List<GenericResponseModel>>(MHRSUrls.BaseUrl, string.Format(MHRSUrls.GetHospitals, provinceIndex, districtIndex, clinicIndex));
            if (hospitalListResponse is null || !hospitalListResponse.Success || hospitalListResponse.Data is null || !hospitalListResponse.Data.Any())
            {
                ConsoleUtil.WriteText("Hastane listesi alınamadı veya boş geldi. Program sonlandırılıyor.", 3000);
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
                try
                {
                    hospitalIndex = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    // FIX: FormatException durumunda döngünün tekrar çalışması için geçersiz değer ata.
                    hospitalIndex = -1;
                }
            } while (hospitalIndex < 0 || hospitalIndex > hospitalList.Count);

            if (hospitalIndex != 0)
            {
                var hospital = hospitalList[hospitalIndex - 1];
                if (hospital.Children != null && hospital.Children.Any())
                {
                    int subHospitalIndex = -1;
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

                        try
                        {
                            subHospitalIndex = Convert.ToInt32(Console.ReadLine());
                        }
                        catch (FormatException)
                        {
                            // FIX: FormatException durumunda iç döngünün tekrar çalışması için geçersiz değer ata.
                            subHospitalIndex = -1;
                        }
                    } while (subHospitalIndex < 0 || subHospitalIndex > hospital.Children.Count);

                    if (subHospitalIndex is 0)
                    {
                        hospitalIndex = hospital.Value;
                    }
                    else
                    {
                        hospitalIndex = hospital.Children[subHospitalIndex - 1].Value;
                    }
                }
                else
                {
                    hospitalIndex = hospital.Value;
                }
            }
            else
            {
                hospitalIndex = -1;
            }

            #endregion

            #region Muayene Yeri Seçim Bölümü
            int placeIndex = -1;
            var placeListResponse = await _client.GetAsync<List<ClinicResponseModel>>(MHRSUrls.BaseUrl, string.Format(MHRSUrls.GetPlaces, hospitalIndex, clinicIndex));
            if (placeListResponse is null || !placeListResponse.Success || placeListResponse.Data is null || !placeListResponse.Data.Any())
            {
                ConsoleUtil.WriteText("Muayene Yeri listesi alınamadı veya boş geldi. Program sonlandırılıyor.", 3000);
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
                try
                {
                    placeIndex = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    // FIX: FormatException durumunda döngünün tekrar çalışması için geçersiz değer ata.
                    placeIndex = -1;
                }
            } while (placeIndex < 0 || placeIndex > placeList.Count);

            if (placeIndex != 0)
                placeIndex = placeList[placeIndex - 1].Value;
            else
                placeIndex = -1;

            #endregion

            #region Doktor Seçim Bölümü
            int doctorIndex = -1;
            var doctorListResponse = await _client.GetAsync<List<GenericResponseModel>>(MHRSUrls.BaseUrl, string.Format(MHRSUrls.GetDoctors, hospitalIndex, clinicIndex));
            if (doctorListResponse is null || !doctorListResponse.Success || doctorListResponse.Data is null || !doctorListResponse.Data.Any())
            {
                ConsoleUtil.WriteText("Doktor listesi alınamadı veya boş geldi. Program sonlandırılıyor.", 3000);
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
                try
                {
                    doctorIndex = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    // FIX: FormatException durumunda döngünün tekrar çalışması için geçersiz değer ata.
                    doctorIndex = -1;
                }
            } while (doctorIndex < 0 || doctorIndex > doctorList.Count);

            if (doctorIndex != 0)
                doctorIndex = doctorList[doctorIndex - 1].Value;
            else
                doctorIndex = -1;

            Console.Clear();
            #endregion

            #region Tarih Seçim Bölümü
            string? startDate = null;
            string? endDate = null;

            ConsoleUtil.WriteText("Tarih girmek istemiyorsanız boş bırakınız (Enter'a basın)...", 0);
            ConsoleUtil.WriteText($"Not: Bitiş tarihi en fazla {DateTime.Now.AddDays(12):dd-MM-yyyy} olabilir.\n", 0);

            do
            {
                Console.Write("Başlangıç tarihi giriniz (Format: Gün-Ay-Yıl örn: 28-04-2025): ");
                string inputDate = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(inputDate))
                {
                    startDate = null;
                    break;
                }

                try
                {
                    var dateArr = inputDate.Split('-').Select(x => Convert.ToInt32(x.Trim())).ToArray();
                    if (dateArr.Length != 3) throw new FormatException("Eksik veya fazla bölüm.");
                    var date = new DateTime(dateArr[2], dateArr[1], dateArr[0]);
                    startDate = date.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                }
                catch (Exception ex) when (ex is FormatException || ex is ArgumentOutOfRangeException || ex is IndexOutOfRangeException)
                {
                    // FIX: Tarih formatı veya geçerliliği hatası (örn: 30-02-2025, AA-12-2025)
                    ConsoleUtil.WriteText("Tarih formatı hatalı. Lütfen tarihi şu şekilde giriniz: Gün-Ay-Yıl (örnek: 28-04-2025)", 0);
                }
                catch (Exception ex)
                {
                    ConsoleUtil.WriteText($"Beklenmedik bir hata oluştu: {ex.Message}. Lütfen tekrar deneyin.", 0);
                }

            } while (true);

            do
            {
                Console.Write("Bitiş tarihi giriniz (Format: Gün-Ay-Yıl örn: 30-04-2025): ");
                string inputDate = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(inputDate))
                {
                    endDate = null;
                    break;
                }

                try
                {
                    var dateArr = inputDate.Split('-').Select(x => Convert.ToInt32(x.Trim())).ToArray();
                    if (dateArr.Length != 3) throw new FormatException("Eksik veya fazla bölüm.");
                    var date = new DateTime(dateArr[2], dateArr[1], dateArr[0]);

                    if (startDate != null && date < DateTime.Parse(startDate))
                    {
                        ConsoleUtil.WriteText("Bitiş tarihi başlangıç tarihinden önce olamaz! Tekrar girin.", 0);
                        continue;
                    }
                    if (date > DateTime.Now.Date.AddDays(13).AddSeconds(-1))
                    {
                        ConsoleUtil.WriteText($"Bitiş tarihi en fazla {DateTime.Now.AddDays(12).ToString("dd-MM-yyyy")} olabilir! Tekrar girin.", 0);
                        continue;
                    }

                    endDate = date.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                }
                catch (Exception ex) when (ex is FormatException || ex is ArgumentOutOfRangeException || ex is IndexOutOfRangeException)
                {
                    // FIX: Tarih formatı veya geçerliliği hatası
                    ConsoleUtil.WriteText("Tarih formatı hatalı. Lütfen tarihi şu şekilde giriniz: Gün-Ay-Yıl (örnek: 28-04-2025)", 0);
                }
                catch (Exception ex)
                {
                    ConsoleUtil.WriteText($"Beklenmedik bir hata oluştu: {ex.Message}. Lütfen tekrar deneyin.", 0);
                }
            } while (true);
            #endregion

            #region Randevu Alım Bölümü
            ConsoleUtil.WriteText("Yapmış olduğunuz seçimler doğrultusunda müsait olan ilk randevu otomatik olarak alınacaktır.", 3000);
            Console.Clear();

            bool appointmentState = false;
            do
            {
                if (TOKEN_END_DATE == default || TOKEN_END_DATE < DateTime.Now)
                {
                    Console.WriteLine("Oturum süresi sona erdi. Giriş bilgileri yenileniyor...");
                    var tokenData = await GetToken(_client);
                    if (tokenData is null || string.IsNullOrEmpty(tokenData.Token))
                    {
                        ConsoleUtil.WriteText("Yeniden giriş yapılırken bir hata meydana geldi! Token alınamadı.", 2000);
                        return;
                    }
                    JWT_TOKEN = tokenData.Token;
                    TOKEN_END_DATE = tokenData.Expiration;
                    _client.AddOrUpdateAuthorizationHeader(JWT_TOKEN);
                    Console.WriteLine("Token başarıyla yenilendi.");
                }

                var slotRequestModel = new SlotRequestModel
                {
                    MhrsHekimId = doctorIndex,
                    MhrsIlId = provinceIndex,
                    MhrsIlceId = districtIndex,
                    MhrsKlinikId = clinicIndex,
                    MhrsKurumId = hospitalIndex,
                    MuayeneYeriId = placeIndex,
                    BaslangicZamani = startDate,
                    BitisZamani = endDate
                };

                Console.WriteLine($"{DateTime.Now:HH:mm:ss} - Uygun randevular kontrol ediliyor...");
                var slot = await GetSlot(_client, slotRequestModel);
                if (slot is null)
                {
                    Console.WriteLine("Şu anda uygun bir randevu bulunamadı. 5 dakika sonra tekrar kontrol edilecek.");
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

                Console.WriteLine($"Müsait randevu bulundu! Tarih: {slot.BaslangicZamani}");
                Console.WriteLine("Randevu alınıyor...");
                appointmentState = await MakeAppointment(_client, appointmentRequestModel);

                if (!appointmentState)
                {
                    Console.WriteLine("Randevu alma işlemi başarısız oldu. 1 dakika içinde tekrar denenecek.");
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }

            } while (!appointmentState);
            #endregion

            Console.WriteLine("\nİşlem tamamlandı. Çıkmak için bir tuşa basın.");
            Console.ReadKey();
        }

        static async Task<JwtTokenModel> GetToken(IClientService client)
        {
            var loginRequestModel = new LoginRequestModel
            {
                KullaniciAdi = TC_NO,
                Parola = SIFRE
            };

            try
            {
                var loginResponse = await client.PostAsync<LoginResponseModel>(MHRSUrls.BaseUrl, MHRSUrls.Login, loginRequestModel);
                if (loginResponse is null || loginResponse.Data is null || string.IsNullOrEmpty(loginResponse.Data.Jwt))
                {
                    ConsoleUtil.WriteText("Giriş başarısız! TC Kimlik numaranızı ve şifrenizi kontrol ederek tekrar deneyin.", 2000);
                    return null; // Token alınamadı
                }

                Console.WriteLine("Giriş başarılı, yeni token alındı.");
                return new() { Token = loginResponse.Data.Jwt, Expiration = JwtTokenUtil.GetTokenExpireTime(loginResponse.Data.Jwt) };
            }
            catch (AggregateException aggrEx)
            {
                ConsoleUtil.WriteText($"Giriş sırasında ağ hatası veya başka bir hata: {aggrEx.InnerException?.Message ?? aggrEx.Message}", 3000);
                return null;
            }
            catch (Exception loginEx)
            {
                ConsoleUtil.WriteText($"Giriş sırasında beklenmedik hata: {loginEx.Message}", 3000);
                return null;
            }
        }

        static async Task<SubSlot> GetSlot(IClientService client, SlotRequestModel slotRequestModel)
        {
            try
            {
                var slotListResponse = await client.PostAsync<List<SlotResponseModel>>(MHRSUrls.BaseUrl, MHRSUrls.GetSlots, slotRequestModel);

                // FIX: .Info özelliği yok. Yanıtın veya Data'nın null olup olmadığını kontrol et.
                if (slotListResponse is null || slotListResponse.Data is null)
                {
                    Console.WriteLine($"Slot bilgisi alınamadı veya geçersiz yanıt alındı.");
                    return null;
                }
                if (!slotListResponse.Data.Any())
                {
                    return null;
                }

                // FIX: LINQ sorgusundaki tür isimleri düzeltildi (HekimSlot -> DoctorSlot vb.)
                var saatSlotList = slotListResponse.Data
                    .SelectMany(d => d.HekimSlotList ?? Enumerable.Empty<DoctorSlot>())
                    .SelectMany(h => h.MuayeneYeriSlotList ?? Enumerable.Empty<PlaceSlot>())
                    .SelectMany(m => m.SaatSlotList ?? Enumerable.Empty<DateSlot>())
                    .ToList();

                if (!saatSlotList.Any())
                {
                    return null;
                }

                var availableSlot = saatSlotList
                    .Where(s => s.Bos)
                    .SelectMany(s => s.SlotList ?? Enumerable.Empty<Models.ResponseModels.Slot>())
                    .Where(sl => sl.Bos)
                    .Select(sl => sl.SubSlot)
                    .FirstOrDefault(sub => sub != null);

                return availableSlot;
            }
            catch (AggregateException aggrEx)
            {
                Console.WriteLine($"Slot alınırken ağ hatası veya başka bir hata: {aggrEx.InnerException?.Message ?? aggrEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Slot alınırken beklenmedik bir hata oluştu: {ex.Message}");
                return null;
            }
        }

        static async Task<bool> MakeAppointment(IClientService client, AppointmentRequestModel appointmentRequestModel)
        {
            try
            {
                var randevuResp = await client.PostSimpleAsync(MHRSUrls.BaseUrl, MHRSUrls.MakeAppointment, appointmentRequestModel);

                if (randevuResp.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine($"Randevu alırken bir problem ile karşılaşıldı! HTTP Status: {randevuResp.StatusCode}. Randevu Tarihi -> {appointmentRequestModel.BaslangicZamani}");
                    return false;
                }

                var message = $"✅ Randevunuz başarıyla alındı!\nTarih: {appointmentRequestModel.BaslangicZamani}\nMHRS sistemine giriş yaparak detayları görüntüleyebilirsiniz.";
                Console.WriteLine(message);
                return true;
            }
            catch (AggregateException aggrEx)
            {
                Console.WriteLine($"Randevu alınırken ağ hatası veya başka bir hata: {aggrEx.InnerException?.Message ?? aggrEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Randevu alınırken beklenmedik bir hata oluştu: {ex.Message}");
                return false;
            }
        }
    }
}