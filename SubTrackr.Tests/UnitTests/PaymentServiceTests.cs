using NUnit.Framework;
using Moq;
using System;
using SubTrackr.Core.Models;
using SubTrackr.Core.Services;
using SubTrackr.Core.Interfaces;
using SubTrackr.Core.Enums;

namespace SubTrackr.Tests.UnitTests
{
    // ============================================
    // PAYMENT SERVICE TESTS
    // ============================================
    [TestFixture]
    public class PaymentServiceTests
    {
        private Mock<IRepository<Payment>> _mockRepo = null!;
        private Mock<INotificationService> _mockNotificationService = null!;
        private PaymentService _paymentService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<Payment>>();
            _mockNotificationService = new Mock<INotificationService>();
            _paymentService = new PaymentService(_mockRepo.Object, _mockNotificationService.Object);
        }

        [Test]
        public void ProcessPayment_ValidAmount_CreatesPayment()
        {
            // Arrange
            var user = new User { Id = "user1", Name = "Test User" };
            var subscription = new BasicSubscription { Id = "sub1", Cost = 10.00m };

            // Act
            var payment = _paymentService.ProcessPayment(user, subscription, 10.00m);

            // Assert
            Assert.IsNotNull(payment);
            Assert.AreEqual("user1", payment.UserId);
            Assert.AreEqual("sub1", payment.SubscriptionId);
            Assert.AreEqual(10.00m, payment.Amount);
            _mockRepo.Verify(r => r.Add(It.IsAny<Payment>()), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-5)]
        [TestCase(-100)]
        public void ProcessPayment_InvalidAmount_ThrowsArgumentException(decimal amount)
        {
            // Arrange
            var user = new User { Id = "user1" };
            var subscription = new BasicSubscription { Id = "sub1" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                _paymentService.ProcessPayment(user, subscription, amount));
        }

        [Test]
        public void RetryFailedPayment_FailedPayment_RetriesPayment()
        {
            // Arrange
            var payment = new Payment 
            { 
                Id = "pay1", 
                Status = PaymentStatus.Failed, 
                RetryCount = 0 
            };

            // Act
            _paymentService.RetryFailedPayment(payment);

            // Assert
            Assert.AreEqual(1, payment.RetryCount);
            _mockRepo.Verify(r => r.Update(payment), Times.Once);
        }

        [Test]
        public void RetryFailedPayment_SuccessfulPayment_ThrowsException()
        {
            // Arrange
            var payment = new Payment { Status = PaymentStatus.Success };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                _paymentService.RetryFailedPayment(payment));
        }

        [Test]
        public void RetryFailedPayment_MaxRetriesReached_ReturnsFalse()
        {
            // Arrange
            var payment = new Payment 
            { 
                Status = PaymentStatus.Failed, 
                RetryCount = 3 
            };

            // Act
            bool result = _paymentService.RetryFailedPayment(payment);

            // Assert
            Assert.IsFalse(result);
        }
    }
}

