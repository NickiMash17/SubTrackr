using System;
using System.Collections.Generic;
using System.Linq;
using SubTrackr.Core.Models;
using SubTrackr.Core.Interfaces;

namespace SubTrackr.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification> _notificationRepository;

        public NotificationService(IRepository<Notification> notificationRepository)
        {
            _notificationRepository = notificationRepository ??
                throw new ArgumentNullException(nameof(notificationRepository));
        }

        public void SendRenewalReminder(User user, SubscriptionBase subscription)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));

            var notification = new Notification
            {
                UserId = user.Id,
                Type = "Renewal",
                Message = $"Your subscription '{subscription.PlanName}' is due for renewal " +
                         $"on {subscription.GetNextRenewalDate():yyyy-MM-dd}. " +
                         $"Amount: ${subscription.CalculateRenewalCost():F2}"
            };

            _notificationRepository.Add(notification);
        }

        public void SendPaymentFailedNotification(User user, Payment payment)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (payment == null) throw new ArgumentNullException(nameof(payment));

            var notification = new Notification
            {
                UserId = user.Id,
                Type = "PaymentFailed",
                Message = $"Payment of ${payment.Amount:F2} failed. " +
                         $"Reason: {payment.FailureReason}. " +
                         $"Please update your payment information."
            };

            _notificationRepository.Add(notification);
        }

        public void SendCancellationConfirmation(User user, SubscriptionBase subscription)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (subscription == null) throw new ArgumentNullException(nameof(subscription));

            var notification = new Notification
            {
                UserId = user.Id,
                Type = "Cancellation",
                Message = $"Your subscription '{subscription.PlanName}' has been " +
                         $"cancelled successfully. Active until {subscription.EndDate:yyyy-MM-dd}."
            };

            _notificationRepository.Add(notification);
        }

        public List<Notification> GetUserNotifications(string userId)
        {
            return _notificationRepository.GetAll()
                .Where(n => n.UserId == userId).ToList();
        }

        public void MarkAsRead(string notificationId)
        {
            var notification = _notificationRepository.GetById(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _notificationRepository.Update(notification);
            }
        }
    }
}