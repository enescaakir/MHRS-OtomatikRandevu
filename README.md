# MHRS Otomatik Randevu
MHRS kullanıcı bilgileriniz ile giriş yaptıktan sonra İl-İlçe-Klinik-Doktor gibi filtrelemeler yaparak aradığınız randevunun müsaitlik durumunu anlık olarak takip edebilir veya randevuyu otomatik olarak alabilirsiniz.

## Neden Buna İhtiyaç Duyuldu?
Devlet hastanelerinde randevu bulmak oldukça zorlaştı, MHRS'nin vatandaşlara sunmuş olduğu randevu müsaitlik bildirim sistemi yeteri kadar hızlı ve sağlıklı çalışmadığı için bu konsol uygulaması geliştirildi.

## Randevu Tarama Sıklığı Nedir?
Aradığınız kriterlere uygun randevu sisteme düştüğünde 5 dakika içerisinde otomatik olarak randevu tarafınıza bildirilir ve alınır.

## Bildirim Sistemi Nasıl İşlemektedir?
Ücretsiz bir şekilde kullanabileceğiniz [Twilio](https://twilio.com) bildirim servisinden hesap oluştururken kullandığınız cep telefonunuza sms gönderilir.

## Bildirim Sistemi Kurulumu
Mevcut uygulamada bildirim sistemi opsiyonel olmadığı için uygulamayı kullanmaya başlamadan önce aşağıdaki adımları mutlaka uygulamanız gerekmektedir.

1-[Twilio](https://twilio.com) hesabı açıp cep telefonu numaranızı onaylayın,\
2-[Konsol sayfası](https://console.twilio.com)na giderek aşağıdaki bilgilerinize ulaşabilirsiniz. Bu bilgileri [NotificationService](https://github.com/kuzudoli/MHRS-OtomatikRandevu/blob/master/MHRS-OtomatikRandevu/Services/NotificationService.cs) sınıfındaki alanlara eklemeniz gerekmektedir.

![Twilio Hesap Bilgileri](https://i.hizliresim.com/tfeswu3.jpg)

<b>NOT: SADECE TWILIO HESABINIZA TANIMLI OLAN ONAYLANMIŞ CEP TELEFONU NUMARANIZA SMS GÖNDERİLEBİLİR.</b>
