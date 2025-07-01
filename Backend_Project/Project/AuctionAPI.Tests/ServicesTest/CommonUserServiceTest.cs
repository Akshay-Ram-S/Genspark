using System;
using System.Threading.Tasks;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using AuctionAPI.Services;
using Moq;
using NUnit.Framework;

namespace AuctionAPI.Tests.ServicesTest
{
    public class CommonUserServiceTests
    {
        private Mock<IRepository<string, User>> _userRepoMock;
        private Mock<IRepository<Guid, Audit>> _auditRepoMock;
        private Mock<IEncryptionService> _encryptionMock;
        private Mock<IFunctionalities> _functionalityMock;
        private CommonUserService _service;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IRepository<string, User>>();
            _auditRepoMock = new Mock<IRepository<Guid, Audit>>(); // ignored
            _encryptionMock = new Mock<IEncryptionService>();
            _functionalityMock = new Mock<IFunctionalities>();

            _service = new CommonUserService(_userRepoMock.Object, _auditRepoMock.Object, _encryptionMock.Object, _functionalityMock.Object);
        }

        [Test]
        public async Task UpdateUser_Success()
        {
            // Arrange
            var email = "test@example.com";
            var oldUser = new User
            {
                Email = email,
                Name = "Old Name",
                Password = "oldpassword",
                UserId = Guid.NewGuid()
            };

            var dto = new UpdateUserDto
            {
                NewPassword = "newpassword",
                CurrentPassword = "oldpassword"
            };

            _userRepoMock.Setup(r => r.Get(email)).ReturnsAsync(oldUser);
            _encryptionMock.Setup(e => e.EncryptData(It.IsAny<EncryptModel>()))
                .ReturnsAsync(new EncryptModel { EncryptedData = "EncryptedPassword" });

            _userRepoMock.Setup(r => r.Update(email, It.IsAny<User>())).ReturnsAsync((string _, User u) => u);

            // Act
            var result = await _service.UpdateUser(email, dto);
            Assert.That(result.Password, Is.EqualTo("EncryptedPassword"));
        }

        [Test]
        public void UpdateUser_Exception()
        {
            // Arrange
            var email = "missing@example.com";
            _userRepoMock.Setup(r => r.Get(email)).ReturnsAsync((User)null);

            var dto = new UpdateUserDto { NewPassword = "Test", CurrentPassword = "pwd" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.UpdateUser(email, dto));
            Assert.That(ex.Message, Is.EqualTo("No User found with email: missing@example.com"));
        }

        [Test]
        public async Task DeleteUser_Success()
        {
            // Arrange
            var email = "user@example.com";
            var user = new User { Email = email, Status = "Active", UserId = Guid.NewGuid() };

            _userRepoMock.Setup(r => r.Get(email)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.Update(email, It.IsAny<User>())).ReturnsAsync((string _, User u) => u);

            // Act
            var result = await _service.DeleteUser(email);

            // Assert
            Assert.That(result.Status, Is.EqualTo("Deleted"));
        }

        [Test]
        public void DeleteUser_Exception()
        {
            // Arrange
            var email = "invalid@example.com";
            _userRepoMock.Setup(r => r.Get(email)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.DeleteUser(email));
            Assert.That(ex.Message, Is.EqualTo("No User found with email: invalid@example.com"));
        }
    }
}
