using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SubTrackr.Core.Models;
using SubTrackr.Core.Services;
using SubTrackr.Core.Repositories;
using SubTrackr.Core.Enums;

namespace SubTrackr.Tests.NonFunctionalTests
{
    // ============================================
    // NON-FUNCTIONAL TESTS: PERFORMANCE
    // ============================================

    [TestFixture]
    public class PerformanceTests
    {
        [Test]
        public void ProcessMultiplePayments_Performance_CompletesInReasonableTime()
        {
            // Arrange
            var paymentRepo = new PaymentRepository();
            var notificationRepo = new NotificationRepository();
            var notificationService = new NotificationService(notificationRepo);
            var paymentService = new PaymentService(paymentRepo, notificationService);

            var user = new User { Id = "perf_user", Name = "Performance Test User" };
            var subscription = new BasicSubscription { Id = "perf_sub", Cost = 9.99m };

            int paymentCount = 100;
            var stopwatch = Stopwatch.StartNew();

            // Act
            for (int i = 0; i < paymentCount; i++)
            {
                paymentService.ProcessPayment(user, subscription, 9.99m);
            }

            stopwatch.Stop();

            // Assert
            Assert.Less(stopwatch.ElapsedMilliseconds, 5000, 
                $"Processing {paymentCount} payments took {stopwatch.ElapsedMilliseconds}ms, which exceeds 5000ms threshold");
            
            var payments = paymentService.GetPaymentHistory(user.Id);
            Assert.AreEqual(paymentCount, payments.Count);
            
            Console.WriteLine($"Performance: {paymentCount} payments processed in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Average: {stopwatch.ElapsedMilliseconds / paymentCount}ms per payment");
        }

        [Test]
        public void GenerateReportForLargeDataset_Performance_CompletesQuickly()
        {
            // Arrange
            var userRepo = new UserRepository();
            var subscriptionRepo = new SubscriptionRepository();
            var paymentRepo = new PaymentRepository();
            var notificationRepo = new NotificationRepository();
            var notificationService = new NotificationService(notificationRepo);
            var paymentService = new PaymentService(paymentRepo, notificationService);
            var reportService = new ReportService(userRepo, subscriptionRepo, paymentRepo);

            // Create user with multiple subscriptions and payments
            var user = new User { Id = "large_data_user", Name = "Large Data User" };
            userRepo.Add(user);

            for (int i = 0; i < 50; i++)
            {
                var subscription = new BasicSubscription 
                { 
                    Id = $"sub_{i}", 
                    UserId = user.Id, 
                    PlanName = $"Plan {i}",
                    Cost = 9.99m,
                    Status = SubscriptionStatus.Active
                };
                subscriptionRepo.Add(subscription);

                // Add payments for each subscription
                for (int j = 0; j < 10; j++)
                {
                    var payment = new Payment
                    {
                        UserId = user.Id,
                        SubscriptionId = subscription.Id,
                        Amount = 9.99m,
                        Status = PaymentStatus.Success,
                        PaymentDate = DateTime.Now.AddDays(-j)
                    };
                    paymentRepo.Add(payment);
                }
            }

            var stopwatch = Stopwatch.StartNew();

            // Act
            var report = reportService.GenerateMonthlyReport(user.Id, DateTime.Now);

            stopwatch.Stop();

            // Assert
            Assert.Less(stopwatch.ElapsedMilliseconds, 1000, 
                $"Report generation took {stopwatch.ElapsedMilliseconds}ms, which exceeds 1000ms threshold");
            Assert.IsNotNull(report);
            Assert.AreEqual(50, report.ActiveSubscriptions.Count);
            
            Console.WriteLine($"Performance: Report generated in {stopwatch.ElapsedMilliseconds}ms for 50 subscriptions and 500 payments");
        }

        [Test]
        public void BulkUserOperations_Performance_ScalesWell()
        {
            // Arrange
            var userRepo = new UserRepository();
            var userService = new UserService(userRepo);
            int userCount = 1000;

            var stopwatch = Stopwatch.StartNew();

            // Act - Create users
            for (int i = 0; i < userCount; i++)
            {
                userService.AddUser($"User {i}", $"user{i}@example.com", UserRole.Customer);
            }

            stopwatch.Stop();
            long createTime = stopwatch.ElapsedMilliseconds;

            // Act - Retrieve all users
            stopwatch.Restart();
            var allUsers = userService.GetAllUsers();
            stopwatch.Stop();
            long retrieveTime = stopwatch.ElapsedMilliseconds;

            // Assert
            Assert.AreEqual(userCount, allUsers.Count);
            Assert.Less(createTime, 2000, $"Creating {userCount} users took {createTime}ms");
            Assert.Less(retrieveTime, 100, $"Retrieving {userCount} users took {retrieveTime}ms");
            
            Console.WriteLine($"Performance: Created {userCount} users in {createTime}ms, retrieved in {retrieveTime}ms");
        }
    }
}

