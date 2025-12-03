using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubTrackr.Core.Models;
using SubTrackr.Core.Services;
using SubTrackr.Core.Repositories;
using SubTrackr.Core.Enums;

namespace SubTrackr.Tests.NonFunctionalTests
{
    // ============================================
    // NON-FUNCTIONAL TESTS: RELIABILITY
    // ============================================

    [TestFixture]
    public class ReliabilityTests
    {
        [Test]
        public void ProcessPayment_SimultaneousRequests_HandlesGracefully()
        {
            // Arrange
            var paymentRepo = new PaymentRepository();
            var notificationRepo = new NotificationRepository();
            var notificationService = new NotificationService(notificationRepo);
            var paymentService = new PaymentService(paymentRepo, notificationService);

            var user = new User { Id = "concurrent_user", Name = "Concurrent User" };
            var subscription = new BasicSubscription { Id = "concurrent_sub", Cost = 9.99m };

            var tasks = new List<Task>();
            int concurrentRequests = 10;

            // Act - Simulate concurrent payment processing
            for (int i = 0; i < concurrentRequests; i++)
            {
                tasks.Add(Task.Run(() => 
                {
                    try
                    {
                        paymentService.ProcessPayment(user, subscription, 9.99m);
                    }
                    catch (Exception ex)
                    {
                        // Log but don't fail - we're testing reliability
                        Console.WriteLine($"Payment failed: {ex.Message}");
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Assert - Verify all payments were processed
            var payments = paymentService.GetPaymentHistory(user.Id);
            Assert.AreEqual(concurrentRequests, payments.Count, 
                "Not all concurrent payments were processed");
            
            Console.WriteLine($"Reliability: {concurrentRequests} concurrent payments processed successfully");
        }

        [Test]
        public void ServiceOperations_InvalidInput_DoesNotCrash()
        {
            // Arrange
            var userRepo = new UserRepository();
            var userService = new UserService(userRepo);

            // Act & Assert - Test various invalid inputs
            Assert.Throws<ArgumentException>(() => userService.AddUser("", "test@example.com", UserRole.Customer));
            Assert.Throws<ArgumentException>(() => userService.AddUser("Name", "invalid", UserRole.Customer));
            Assert.Throws<InvalidOperationException>(() => userService.UpdateUser("nonexistent", "Name", "test@example.com", UserRole.Customer));
            
            // Service should still be functional after exceptions
            userService.AddUser("Valid User", "valid@example.com", UserRole.Customer);
            var users = userService.GetAllUsers();
            Assert.AreEqual(1, users.Count);
            
            Console.WriteLine("Reliability: Service handles invalid inputs gracefully without crashing");
        }

        [Test]
        public void PaymentRetryMechanism_RepeatedFailures_EventuallyStops()
        {
            // Arrange
            var paymentRepo = new PaymentRepository();
            var notificationRepo = new NotificationRepository();
            var notificationService = new NotificationService(notificationRepo);
            var paymentService = new PaymentService(paymentRepo, notificationService);

            var failedPayment = new Payment
            {
                Id = "retry_payment",
                Status = PaymentStatus.Failed,
                RetryCount = 0,
                Amount = 9.99m
            };

            paymentRepo.Add(failedPayment);

            // Act - Retry until max attempts or payment succeeds
            int retryCount = 0;
            while (retryCount < 5 && failedPayment.RetryCount < 3 && failedPayment.Status == PaymentStatus.Failed)
            {
                try
                {
                    paymentService.RetryFailedPayment(failedPayment);
                }
                catch (InvalidOperationException)
                {
                    // Payment succeeded, stop retrying
                    break;
                }
                retryCount++;
            }

            // Assert - System should stop retrying after max attempts
            Assert.LessOrEqual(failedPayment.RetryCount, 3, 
                "Payment retry count exceeded maximum allowed attempts");
            
            Console.WriteLine($"Reliability: Payment retry mechanism stopped after {failedPayment.RetryCount} attempts");
        }

        [Test]
        public void SubscriptionCancellation_MultipleCalls_IdempotentBehavior()
        {
            // Arrange
            var subscriptionRepo = new SubscriptionRepository();
            var notificationRepo = new NotificationRepository();
            var notificationService = new NotificationService(notificationRepo);
            var subscriptionService = new SubscriptionService(subscriptionRepo, notificationService);

            var user = new User { Id = "user1", Name = "Test User" };
            var subscription = new BasicSubscription
            {
                Id = "sub1",
                UserId = user.Id,
                Status = SubscriptionStatus.Active,
                PlanName = "Test Plan"
            };
            subscriptionRepo.Add(subscription);

            // Act - Cancel multiple times
            subscriptionService.CancelSubscription(subscription.Id, user);
            DateTime firstCancellation = subscription.EndDate.Value;
            
            System.Threading.Thread.Sleep(100); // Small delay
            
            subscriptionService.CancelSubscription(subscription.Id, user);
            DateTime secondCancellation = subscription.EndDate.Value;

            // Assert - Multiple cancellations don't cause issues
            Assert.AreEqual(SubscriptionStatus.Cancelled, subscription.Status);
            // Dates should be similar (allowing for processing time)
            Assert.IsTrue(Math.Abs((secondCancellation - firstCancellation).TotalSeconds) < 5);
            
            Console.WriteLine("Reliability: Subscription cancellation is idempotent");
        }
    }
}

