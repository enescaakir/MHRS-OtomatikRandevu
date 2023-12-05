namespace MHRS_OtomatikRandevu.Services.Abstracts
{
    public interface INotificationService
    {
        public Task SendNotification(string message);
    }
}