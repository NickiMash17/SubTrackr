using System.Collections.Generic;
using SubTrackr.Core.Models;

namespace SubTrackr.Core.Interfaces
{
    public interface INotificationService
    {
        void SendRenewalReminder(User user, SubscriptionBase subscription);
        void SendPaymentFailedNotification(User user, Payment payment);
        void SendCancellationConfirmation(User user, SubscriptionBase subscription);
        List<Notification> GetUserNotifications(string userId);
        void MarkAsRead(string notificationId);
    }
}