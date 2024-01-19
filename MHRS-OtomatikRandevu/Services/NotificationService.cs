using MHRS_OtomatikRandevu.Services.Abstracts;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Configuration;
using System.Collections.Specialized;
using MHRS_OtomatikRandevu.Utils;

namespace MHRS_OtomatikRandevu.Services
{
    public class NotificationService : INotificationService
    {
        public string? TWILIO_ACCOUNT_SID = "";
        public string? TWILIO_AUTH_TOKEN = "";
        public string? TWILIO_PHONE_NUMBER = "";
        public string? PHONE_NUMBER = "";

        public NotificationService()
        {
            if(!HasConfig())
            {
                TWILIO_ACCOUNT_SID = ConfigurationManager.AppSettings.Get(nameof(TWILIO_ACCOUNT_SID));
                TWILIO_AUTH_TOKEN = ConfigurationManager.AppSettings.Get(nameof(TWILIO_AUTH_TOKEN));
                TWILIO_PHONE_NUMBER = ConfigurationManager.AppSettings.Get(nameof(TWILIO_PHONE_NUMBER));
                PHONE_NUMBER = ConfigurationManager.AppSettings.Get(nameof(PHONE_NUMBER));
            }

            if (!HasConfig())
            {
                ConsoleUtil.WriteText("Eğer bildirim sistemini kullanmak istiyorsanız lütfen config dosyasındaki değerleri doldurun ve tekrar başlatın.", 2000);
            }
            
            TwilioClient.Init(TWILIO_ACCOUNT_SID, TWILIO_AUTH_TOKEN);
        }

        private bool HasConfig()
        {
            return string.IsNullOrEmpty(TWILIO_ACCOUNT_SID) || string.IsNullOrEmpty(TWILIO_AUTH_TOKEN) 
                                                            || string.IsNullOrEmpty(TWILIO_PHONE_NUMBER) 
                                                            || string.IsNullOrEmpty(PHONE_NUMBER);
        }

        public async Task SendNotification(string message)
        {
            await MessageResource.CreateAsync(
                from: new Twilio.Types.PhoneNumber(TWILIO_PHONE_NUMBER),
                to: new Twilio.Types.PhoneNumber(PHONE_NUMBER),
                body: message
            );
        }
    }
}