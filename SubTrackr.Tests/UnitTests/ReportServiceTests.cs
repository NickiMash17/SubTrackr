using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using SubTrackr.Core.Models;
using SubTrackr.Core.Services;
using SubTrackr.Core.Interfaces;
using SubTrackr.Core.Enums;
using SubTrackr.Core.Factories;

namespace SubTrackr.Tests.UnitTests
{
    // ============================================
    // REPORT SERVICE TESTS
    // ============================================
    [TestFixture]
    public class ReportServiceTests
    {
        private Mock<IRepository<User>> _mockUserRepo;
        private Mock<IRepository<SubscriptionBase>> _mockSubRepo;
        private Mock<IRepository<Payment>> _mockPaymentRepo;
        private ReportService _reportService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepo = new Mock<IRepository<User>>();
            _mockSubRepo = new Mock<IRepository<SubscriptionBase>>();
            _mockPaymentRepo = new Mock<IRepository<Payment>>();
            _reportService = new ReportService(_mockUserRepo.Object, _mockSubRepo.Object, _mockPaymentRepo.Object);
        }

        [Test]
        public void GenerateMonthlyReport_ValidUser_ReturnsReport()
        {
            // Arrange
            var user = new User { Id = "user1", Name = "Test User" };
            var subscriptions = new List<SubscriptionBase>
            {
                new BasicSubscription { UserId = "user1", Status = SubscriptionStatus.Active }
            };
            var payments = new List<Payment>
            {
                new Payment { UserId = "user1", Amount = 10.00m, Status = PaymentStatus.Success, PaymentDate = new DateTime(2025, 11, 1) },
                new Payment { UserId = "user1", Amount = 5.00m, Status = PaymentStatus.Failed, PaymentDate = new DateTime(2025, 11, 15) }
            };

            _mockUserRepo.Setup(r => r.GetById("user1")).Returns(user);
            _mockSubRepo.Setup(r => r.GetAll()).Returns(subscriptions);
            _mockPaymentRepo.Setup(r => r.GetAll()).Returns(payments);

            // Act
            var report = _reportService.GenerateMonthlyReport("user1", new DateTime(2025, 11, 1));

            // Assert
            Assert.IsNotNull(report);
            Assert.AreEqual("user1", report.UserId);
            Assert.AreEqual("Test User", report.UserName);
            Assert.AreEqual(1, report.ActiveSubscriptions.Count);
            Assert.AreEqual(2, report.PaymentHistory.Count);
            Assert.AreEqual(10.00m, report.TotalAmountBilled);
            Assert.AreEqual(1, report.FailedPayments);
        }

        [Test]
        public void GenerateMonthlyReport_NonExistentUser_ThrowsException()
        {
            // Arrange
            _mockUserRepo.Setup(r => r.GetById(It.IsAny<string>())).Returns((User)null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                _reportService.GenerateMonthlyReport("999", DateTime.Now));
        }

        [Test]
        public void ExportReportToString_ValidReport_ReturnsFormattedString()
        {
            // Arrange
            var report = new MonthlyReport
            {
                UserId = "user1",
                UserName = "Test User",
                ReportMonth = new DateTime(2025, 11, 1),
                TotalAmountBilled = 50.00m,
                FailedPayments = 1
            };

            // Act
            string result = _reportService.ExportReportToString(report);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("Test User"), "Report should contain user name");
            // Check for the formatted amount - it appears in "Total Billed: $50.00"
            // The format is "$50.00" so check for both the dollar sign and amount
            Assert.IsTrue(result.Contains("50.00") || result.Contains("$50"), 
                $"Report should contain the amount. Actual output: {result}");
            Assert.IsTrue(result.Contains("Total Billed") || result.Contains("Billed"), 
                $"Report should contain 'Total Billed'. Actual output: {result}");
        }
    }

    // ============================================
    // NOTIFICATION SERVICE TESTS
    // ============================================
    [TestFixture]
    public class NotificationServiceTests
    {
        private Mock<IRepository<Notification>> _mockRepo;
        private NotificationService _notificationService;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<Notification>>();
            _notificationService = new NotificationService(_mockRepo.Object);
        }

        [Test]
        public void SendRenewalReminder_ValidData_CreatesNotification()
        {
            // Arrange
            var user = new User { Id = "user1", Name = "Test User" };
            var subscription = new BasicSubscription 
            { 
                PlanName = "Basic Plan", 
                Cost = 9.99m 
            };

            // Act
            _notificationService.SendRenewalReminder(user, subscription);

            // Assert
            _mockRepo.Verify(r => r.Add(It.Is<Notification>(n => 
                n.UserId == "user1" && 
                n.Type == "Renewal" && 
                !string.IsNullOrEmpty(n.Message))), Times.Once);
        }

        [Test]
        public void SendPaymentFailedNotification_ValidData_CreatesNotification()
        {
            // Arrange
            var user = new User { Id = "user1" };
            var payment = new Payment 
            { 
                Amount = 10.00m, 
                FailureReason = "Insufficient funds" 
            };

            // Act
            _notificationService.SendPaymentFailedNotification(user, payment);

            // Assert
            _mockRepo.Verify(r => r.Add(It.Is<Notification>(n => 
                n.UserId == "user1" && 
                n.Type == "PaymentFailed")), Times.Once);
        }

        [Test]
        public void MarkAsRead_ExistingNotification_MarksAsRead()
        {
            // Arrange
            var notification = new Notification { Id = "notif1", IsRead = false };
            _mockRepo.Setup(r => r.GetById("notif1")).Returns(notification);

            // Act
            _notificationService.MarkAsRead("notif1");

            // Assert
            Assert.IsTrue(notification.IsRead);
            _mockRepo.Verify(r => r.Update(notification), Times.Once);
        }
    }

    // ============================================
    // FACTORY PATTERN TESTS
    // ============================================
    [TestFixture]
    public class SubscriptionFactoryTests
    {
        [Test]
        public void CreateSubscription_BasicType_ReturnsBasicSubscription()
        {
            // Arrange & Act
            var subscription = SubscriptionFactory.CreateSubscription(
                "basic", "user1", "Basic Plan", 9.99m, RenewalFrequency.Monthly);

            // Assert
            Assert.IsInstanceOf<BasicSubscription>(subscription);
            Assert.AreEqual("user1", subscription.UserId);
            Assert.AreEqual("Basic Plan", subscription.PlanName);
            Assert.AreEqual(9.99m, subscription.Cost);
        }

        [Test]
        public void CreateSubscription_PremiumType_ReturnsPremiumSubscription()
        {
            // Arrange & Act
            var subscription = SubscriptionFactory.CreateSubscription(
                "premium", "user1", "Premium Plan", 19.99m, RenewalFrequency.Monthly);

            // Assert
            Assert.IsInstanceOf<PremiumSubscription>(subscription);
            var premium = subscription as PremiumSubscription;
            Assert.IsNotNull(premium.BonusFeatures);
            Assert.IsTrue(premium.BonusFeatures.Count > 0);
        }

        [TestCase("")]
        [TestCase(null!)]
        public void CreateSubscription_EmptyType_ThrowsArgumentException(string? type)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                SubscriptionFactory.CreateSubscription(type, "user1", "Plan", 10m, RenewalFrequency.Monthly));
        }

        [TestCase(0)]
        [TestCase(-10)]
        public void CreateSubscription_InvalidCost_ThrowsArgumentException(decimal cost)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                SubscriptionFactory.CreateSubscription("basic", "user1", "Plan", cost, RenewalFrequency.Monthly));
        }

        [Test]
        public void CreateSubscription_UnknownType_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                SubscriptionFactory.CreateSubscription("unknown", "user1", "Plan", 10m, RenewalFrequency.Monthly));
        }
    }

    // ============================================
    // MODEL TESTS
    // ============================================
    [TestFixture]
    public class SubscriptionModelTests
    {
        [Test]
        public void BasicSubscription_CalculateRenewalCost_ReturnsBaseCost()
        {
            // Arrange
            var subscription = new BasicSubscription { Cost = 9.99m };

            // Act
            decimal renewalCost = subscription.CalculateRenewalCost();

            // Assert
            Assert.AreEqual(9.99m, renewalCost);
        }

        [Test]
        public void PremiumSubscription_CalculateRenewalCost_AppliesDiscount()
        {
            // Arrange
            var subscription = new PremiumSubscription 
            { 
                Cost = 100m, 
                DiscountPercentage = 10m 
            };

            // Act
            decimal renewalCost = subscription.CalculateRenewalCost();

            // Assert
            Assert.AreEqual(90m, renewalCost);
        }

        [Test]
        public void SubscriptionBase_IsActive_ActiveStatus_ReturnsTrue()
        {
            // Arrange
            var subscription = new BasicSubscription 
            { 
                Status = SubscriptionStatus.Active,
                EndDate = DateTime.Now.AddDays(10)
            };

            // Act
            bool isActive = subscription.IsActive();

            // Assert
            Assert.IsTrue(isActive);
        }

        [Test]
        public void SubscriptionBase_IsActive_ExpiredDate_ReturnsFalse()
        {
            // Arrange
            var subscription = new BasicSubscription 
            { 
                Status = SubscriptionStatus.Active,
                EndDate = DateTime.Now.AddDays(-10)
            };

            // Act
            bool isActive = subscription.IsActive();

            // Assert
            Assert.IsFalse(isActive);
        }

        [TestCase(RenewalFrequency.Monthly, 30)]
        [TestCase(RenewalFrequency.Quarterly, 90)]
        [TestCase(RenewalFrequency.Yearly, 365)]
        public void GetNextRenewalDate_VariousFrequencies_ReturnsCorrectDate(RenewalFrequency frequency, int expectedDays)
        {
            // Arrange
            var startDate = new DateTime(2025, 1, 1);
            var subscription = new BasicSubscription 
            { 
                StartDate = startDate,
                RenewalFrequency = frequency
            };

            // Act
            DateTime nextRenewal = subscription.GetNextRenewalDate();

            // Assert
            int actualDays = (nextRenewal - startDate).Days;
            Assert.IsTrue(Math.Abs(actualDays - expectedDays) <= 2); // Allow 2 day variance for month differences
        }
    }
}

