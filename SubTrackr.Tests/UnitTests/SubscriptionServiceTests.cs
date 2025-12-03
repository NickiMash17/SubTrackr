using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using SubTrackr.Core.Models;
using SubTrackr.Core.Services;
using SubTrackr.Core.Interfaces;
using SubTrackr.Core.Enums;

namespace SubTrackr.Tests.UnitTests
{
    // ============================================
    // SUBSCRIPTION SERVICE TESTS
    // ============================================
    [TestFixture]
    public class SubscriptionServiceTests
    {
        private Mock<IRepository<SubscriptionBase>> _mockRepo;
        private Mock<INotificationService> _mockNotificationService;
        private SubscriptionService _subscriptionService;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<SubscriptionBase>>();
            _mockNotificationService = new Mock<INotificationService>();
            _subscriptionService = new SubscriptionService(_mockRepo.Object, _mockNotificationService.Object);
        }

        [Test]
        public void CreateSubscription_BasicType_CreatesBasicSubscription()
        {
            // Arrange
            string type = "basic";
            string userId = "user1";
            string planName = "Basic Plan";
            decimal cost = 9.99m;

            // Act
            _subscriptionService.CreateSubscription(type, userId, planName, cost, RenewalFrequency.Monthly);

            // Assert
            _mockRepo.Verify(r => r.Add(It.IsAny<BasicSubscription>()), Times.Once);
        }

        [Test]
        public void CreateSubscription_PremiumType_CreatesPremiumSubscription()
        {
            // Arrange
            string type = "premium";
            string userId = "user1";
            string planName = "Premium Plan";
            decimal cost = 19.99m;

            // Act
            _subscriptionService.CreateSubscription(type, userId, planName, cost, RenewalFrequency.Monthly);

            // Assert
            _mockRepo.Verify(r => r.Add(It.IsAny<PremiumSubscription>()), Times.Once);
        }

        [Test]
        public void CancelSubscription_ValidSubscription_CancelsAndNotifies()
        {
            // Arrange
            var subscription = new BasicSubscription 
            { 
                Id = "sub1", 
                UserId = "user1", 
                Status = SubscriptionStatus.Active 
            };
            var user = new User { Id = "user1", Name = "Test User" };
            
            _mockRepo.Setup(r => r.GetById("sub1")).Returns(subscription);

            // Act
            _subscriptionService.CancelSubscription("sub1", user);

            // Assert
            Assert.AreEqual(SubscriptionStatus.Cancelled, subscription.Status);
            Assert.IsNotNull(subscription.EndDate);
            _mockRepo.Verify(r => r.Update(subscription), Times.Once);
            _mockNotificationService.Verify(n => n.SendCancellationConfirmation(user, subscription), Times.Once);
        }

        [Test]
        public void RenewSubscription_ValidSubscription_RenewsSuccessfully()
        {
            // Arrange
            var subscription = new BasicSubscription 
            { 
                Id = "sub1", 
                EndDate = DateTime.Now.AddDays(-1),
                Status = SubscriptionStatus.Expired 
            };
            
            _mockRepo.Setup(r => r.GetById("sub1")).Returns(subscription);

            // Act
            _subscriptionService.RenewSubscription("sub1");

            // Assert
            Assert.AreEqual(SubscriptionStatus.Active, subscription.Status);
            Assert.IsTrue(subscription.EndDate > DateTime.Now);
            _mockRepo.Verify(r => r.Update(subscription), Times.Once);
        }
    }
}

