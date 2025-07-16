# MHRS Otomatik Randevu Sistemi

MHRS (Merkezi Hekim Randevu Sistemi) üzerinden otomatik randevu alma işlemini gerçekleştiren C# konsol uygulamasıdır. Uygulama, belirlenen kriterlere uygun ilk müsait randevuyu otomatik olarak alır.

## ✨ Özellikler

- **Otomatik Giriş**: TC Kimlik numarası ve şifre ile MHRS sistemine giriş
- **Akıllı Seçim**: İl, ilçe, klinik, hastane, muayene yeri ve doktor seçimi
- **Tarih Filtreleme**: Belirli tarih aralığında randevu arama
- **Otomatik Token Yenileme**: Oturum süresi dolduğunda otomatik token yenileme
- **Sürekli İzleme**: Randevu bulunana kadar 5 dakika aralıklarla kontrol
- **İstanbul Özel Bölge Desteği**: İstanbul için Avrupa/Anadolu yakası seçimi
- **Esnek Seçim**: "Farketmez" seçeneği ile geniş arama imkanı

## 🚀 Kurulum

### Gereksinimler

- Windows 10/11
- Geçerli MHRS hesabı (TC Kimlik numarası ve şifre)

### Yöntem 1: Hızlı Çalıştır ile Çalıştırma (Önerilen)

1. **İndir ve Çalıştır:**
   ```
   1. GitHub'dan projeyi indirin veya klonlayın
   2. `HizliCalistir` klasörüne gidin
   3. `MHRS-OtomatikRandevu.exe` dosyasını çift tıklayın
   ```

2. **Alternatif Çalıştırma:**
   ```bash
   # Komut satırından çalıştırma
   cd HizliCalistir
   MHRS-OtomatikRandevu.exe
   ```

**Not:** Hızlı Çalıştır dosyası ile herhangi bir kuruluma gerek yoktur. Direkt çalıştırabilirsiniz.

### Yöntem 2: Kaynak Koddan Derleme (Geliştiriciler için)

1. **Gereksinimler:**
   - .NET 6.0 SDK veya daha yeni sürüm

2. **Projeyi Klonlama:**
   ```bash
   git clone https://github.com/enescaakir/MHRS-OtomatikRandevu
   cd MHRS-OtomatikRandevu
   ```

3. **Derleme ve Çalıştırma:**
   ```bash
   dotnet build
   dotnet run
   ```

## 📋 Kullanım

### 1. Giriş Bilgileri
Uygulama başlatıldığında:
- TC Kimlik numaranızı girin
- MHRS şifrenizi girin

### 2. Konum Seçimi
- İl seçimi (1-81 plaka kodu)
- İstanbul için özel bölge seçimi:
  - 0: İstanbul (Genel)
  - 1: İstanbul (Avrupa)
  - 2: İstanbul (Anadolu)
- İlçe seçimi (0: Farketmez)

### 3. Sağlık Hizmeti Seçimi
- Klinik seçimi (örn: Dahiliye, Kardiyoloji)
- Hastane seçimi (0: Farketmez)
- Muayene yeri seçimi (0: Farketmez)
- Doktor seçimi (0: Farketmez)

### 4. Tarih Aralığı (Opsiyonel)
- Başlangıç tarihi (Format: GG-AA-YYYY)
- Bitiş tarihi (Format: GG-AA-YYYY)
- Boş bırakılabilir (tüm müsait tarihler)

### 5. Otomatik Randevu Alma
Sistem belirlenen kriterlere uygun ilk müsait randevuyu otomatik olarak alır.

## 🎯 Kullanım Senaryoları

### Hızlı Randevu Alma
```
İl: 34 (İstanbul)
İlçe: 0 (Farketmez)
Klinik: Dahiliye
Hastane: 0 (Farketmez)
Doktor: 0 (Farketmez)
Tarih: Boş (En yakın tarih)
```

### Belirli Tarih Aralığında
```
Başlangıç: 15-04-2025
Bitiş: 20-04-2025
```

### Belirli Doktor
```
Doktor: Dr. Ahmet Yılmaz
Tarih: Boş (Doktorun müsait olduğu ilk tarih)
```

## ⚠️ Önemli Notlar

- **Yasal Sorumluluk**: Bu uygulama kişisel kullanım içindir
- **Tarih Sınırı**: Maksimum 12 gün sonrasına kadar randevu alınabilir
- **Bekleme Süresi**: Randevu bulunamazsa 5 dakika bekler
- **Token Süresi**: Otomatik olarak yenilenir
- **Hata Durumu**: Randevu alma başarısız olursa 1 dakika bekler

## 🔄 Çalışma Mantığı

1. **Kimlik Doğrulama**: MHRS sistemine giriş
2. **Seçim Süreci**: Kullanıcı tercihleri alınır
3. **Döngüsel Kontrol**: 5 dakika aralıklarla müsait randevu aranır
4. **Otomatik Alma**: İlk müsait randevu otomatik alınır
5. **Sonuç**: Başarılı randevu bilgisi gösterilir

## 📱 Ekran Çıktıları

```
MHRS Otomatik Randevu Sistemine Hoşgeldiniz.
TC: 12345678901
Şifre: ********
Giriş Yapılıyor...
Giriş başarılı, yeni token alındı.

-------------------------------------------
1-ADANA
2-ADIYAMAN
...
34-İSTANBUL
-------------------------------------------
Randevu almak istediğiniz ilin plaka kodunu giriniz: 34
```

## 🐛 Sorun Giderme

### Giriş Sorunları
- TC Kimlik numarası ve şifre kontrolü
- İnternet bağlantısı kontrolü
- MHRS sisteminin aktif olup olmadığı

### Token Sorunları
- Otomatik yenileme aktif
- Manual restart gerekebilir

### Randevu Bulunamama
- Tarih aralığını genişletin
- "Farketmez" seçeneklerini kullanın
- Farklı klinik/hastane deneyin

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için `LICENSE` dosyasına bakın.

## 🔗 İletişim

**Geliştirici**: Enes ÇAKIR     
[**GitHub**](https://github.com/enescaakir) | [**Linkedin**](https://www.linkedin.com/in/enescaakir/)

---

**UYARI:** Bu uygulama eğitim ve kişisel kullanım amacıyla geliştirilmiştir. MHRS sisteminin Terms of Service'ini ihlal etmeyecek şekilde kullanın.

---

⭐ Beğendiyseniz yıldızlamayı unutmayın!