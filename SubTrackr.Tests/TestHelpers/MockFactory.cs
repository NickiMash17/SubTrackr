using System;
using System.Collections.Generic;
using SubTrackr.Core.Models;
using SubTrackr.Core.Enums;

namespace SubTrackr.Tests.TestHelpers
{
    /// <summary>
    /// Factory class for creating mock/test data objects
    /// </summary>
    public static class MockFactory
    {
        public static User CreateUser(string? id = null, string name = "Test User", string email = "test@example.com", UserRole role = UserRole.Customer)
        {
            return new User
            {
                Id = id ?? Guid.NewGuid().ToString(),
                Name = name,
                Email = email,
                Role = role,
                CreatedDate = DateTime.Now,
                IsActive = true
            };
        }

        public static BasicSubscription CreateBasicSubscription(string? id = null, string userId = "user1", string planName = "Basic Plan", decimal cost = 9.99m)
        {
            return new BasicSubscription
            {
                Id = id ?? Guid.NewGuid().ToString(),
                UserId = userId,
                PlanName = planName,
                Cost = cost,
                Status = SubscriptionStatus.Active,
                RenewalFrequency = RenewalFrequency.Monthly,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1)
            };
        }

        public static PremiumSubscription CreatePremiumSubscription(string? id = null, string userId = "user1", string planName = "Premium Plan", decimal cost = 19.99m)
        {
            return new PremiumSubscription
            {
                Id = id ?? Guid.NewGuid().ToString(),
                UserId = userId,
                PlanName = planName,
                Cost = cost,
                Status = SubscriptionStatus.Active,
                RenewalFrequency = RenewalFrequency.Monthly,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                DiscountPercentage = 10m,
                BonusFeatures = new List<string> { "Feature 1", "Feature 2" }
            };
        }

        public static Payment CreatePayment(string? id = null, string userId = "user1", string subscriptionId = "sub1", decimal amount = 10.00m, PaymentStatus status = PaymentStatus.Success)
        {
            return new Payment
            {
                Id = id ?? Guid.NewGuid().ToString(),
                UserId = userId,
                SubscriptionId = subscriptionId,
                Amount = amount,
                Status = status,
                PaymentDate = DateTime.Now,
                TransactionReference = Guid.NewGuid().ToString(),
                RetryCount = 0
            };
        }

        public static Notification CreateNotification(string? id = null, string userId = "user1", string type = "Test", string message = "Test notification")
        {
            return new Notification
            {
                Id = id ?? Guid.NewGuid().ToString(),
                UserId = userId,
                Type = type,
                Message = message,
                CreatedDate = DateTime.Now,
                IsRead = false
            };
        }

        public static MonthlyReport CreateMonthlyReport(string userId = "user1", string userName = "Test User", DateTime? reportMonth = null)
        {
            return new MonthlyReport
            {
                UserId = userId,
                UserName = userName,
                ReportMonth = reportMonth ?? DateTime.Now,
                ActiveSubscriptions = new List<SubscriptionBase>(),
                PaymentHistory = new List<Payment>(),
                TotalAmountBilled = 0m,
                FailedPayments = 0
            };
        }
    }
}

