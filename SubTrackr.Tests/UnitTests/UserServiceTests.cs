using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using SubTrackr.Core.Models;
using SubTrackr.Core.Services;
using SubTrackr.Core.Interfaces;
using SubTrackr.Core.Enums;

namespace SubTrackr.Tests.UnitTests
{
    // ============================================
    // USER SERVICE TESTS
    // ============================================
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IRepository<User>> _mockRepo = null!;
        private UserService _userService = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository<User>>();
            _userService = new UserService(_mockRepo.Object);
        }

        [Test]
        public void AddUser_ValidData_AddsUserSuccessfully()
        {
            // Arrange
            string name = "John Doe";
            string email = "john@example.com";
            UserRole role = UserRole.Customer;

            // Act
            _userService.AddUser(name, email, role);

            // Assert
            _mockRepo.Verify(r => r.Add(It.Is<User>(u => 
                u.Name == name && 
                u.Email == email && 
                u.Role == role)), Times.Once);
        }

        [Test]
        public void AddUser_EmptyName_ThrowsArgumentException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentException>(() => 
                _userService.AddUser("", "test@example.com", UserRole.Customer));
        }

        [Test]
        public void AddUser_InvalidEmail_ThrowsArgumentException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentException>(() => 
                _userService.AddUser("John", "invalidemail", UserRole.Customer));
        }

        [Test]
        public void UpdateUser_ExistingUser_UpdatesSuccessfully()
        {
            // Arrange
            var user = new User { Id = "1", Name = "Old Name", Email = "old@example.com", Role = UserRole.Customer };
            _mockRepo.Setup(r => r.GetById("1")).Returns(user);

            // Act
            _userService.UpdateUser("1", "New Name", "new@example.com", UserRole.Admin);

            // Assert
            _mockRepo.Verify(r => r.Update(It.Is<User>(u => 
                u.Name == "New Name" && 
                u.Email == "new@example.com" && 
                u.Role == UserRole.Admin)), Times.Once);
        }

        [Test]
        public void UpdateUser_NonExistentUser_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetById(It.IsAny<string>())).Returns<User>(default!);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                _userService.UpdateUser("999", "Name", "email@test.com", UserRole.Customer));
        }

        [Test]
        public void RemoveUser_ValidId_RemovesUser()
        {
            // Arrange
            string userId = "1";

            // Act
            _userService.RemoveUser(userId);

            // Assert
            _mockRepo.Verify(r => r.Delete(userId), Times.Once);
        }
    }
}

