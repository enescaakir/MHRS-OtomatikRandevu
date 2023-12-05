using MHRS_OtomatikRandevu.Services.Abstracts;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace MHRS_OtomatikRandevu.Services
{
    public class NotificationService : INotificationService
    {
        public const string TWILIO_ACCOUNT_SID = "TWILIO_ACCOUNT_SID";
        public const string TWILIO_AUTH_TOKEN = "TWILIO_AUTH_TOKEN";
        public const string TWILIO_PHONE_NUMBER = "TWILIO_PHONE_NUMBER";
        public const string PHONE_NUMBER = "PHONE_NUMBER"; //Format -> +90XXXyyyZZww

        public NotificationService()
        {
            TwilioClient.Init(TWILIO_ACCOUNT_SID, TWILIO_AUTH_TOKEN);
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