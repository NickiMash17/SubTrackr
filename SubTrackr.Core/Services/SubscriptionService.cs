using System;
using System.Collections.Generic;
using System.Linq;
using SubTrackr.Core.Models;
using SubTrackr.Core.Interfaces;
using SubTrackr.Core.Factories;
using SubTrackr.Core.Enums;

namespace SubTrackr.Core.Services
{
    public class SubscriptionService
    {
        private readonly IRepository<SubscriptionBase> _subscriptionRepository;
        private readonly INotificationService _notificationService;

        public SubscriptionService(
            IRepository<SubscriptionBase> subscriptionRepository,
            INotificationService notificationService)
        {
            _subscriptionRepository = subscriptionRepository ??
                throw new ArgumentNullException(nameof(subscriptionRepository));
            _notificationService = notificationService ??
                throw new ArgumentNullException(nameof(notificationService));
        }

        public void CreateSubscription(string type, string userId, string planName,
            decimal cost, RenewalFrequency frequency)
        {
            var subscription = SubscriptionFactory.CreateSubscription(
                type, userId, planName, cost, frequency);
            _subscriptionRepository.Add(subscription);
        }

        public void UpdateSubscription(string subscriptionId, string planName, decimal cost)
        {
            if (cost <= 0)
                throw new ArgumentException("Cost must be greater than zero");

            var subscription = _subscriptionRepository.GetById(subscriptionId);
            if (subscription == null)
                throw new InvalidOperationException(
                    $"Subscription with ID {subscriptionId} not found");

            subscription.PlanName = planName;
            subscription.Cost = cost;

            _subscriptionRepository.Update(subscription);
        }

        public void CancelSubscription(string subscriptionId, User user)
        {
            var subscription = _subscriptionRepository.GetById(subscriptionId);
            if (subscription == null)
                throw new InvalidOperationException(
                    $"Subscription with ID {subscriptionId} not found");

            subscription.Status = SubscriptionStatus.Cancelled;
            subscription.EndDate = DateTime.Now;

            _subscriptionRepository.Update(subscription);
            _notificationService.SendCancellationConfirmation(user, subscription);
        }

        public void RenewSubscription(string subscriptionId)
        {
            var subscription = _subscriptionRepository.GetById(subscriptionId);
            if (subscription == null)
                throw new InvalidOperationException(
                    $"Subscription with ID {subscriptionId} not found");

            subscription.EndDate = subscription.GetNextRenewalDate();
            subscription.Status = SubscriptionStatus.Active;

            _subscriptionRepository.Update(subscription);
        }

        public List<SubscriptionBase> GetUserSubscriptions(string userId)
        {
            return _subscriptionRepository.GetAll()
                .Where(s => s.UserId == userId).ToList();
        }

        public List<SubscriptionBase> GetAllSubscriptions()
        {
            return _subscriptionRepository.GetAll();
        }
    }
}