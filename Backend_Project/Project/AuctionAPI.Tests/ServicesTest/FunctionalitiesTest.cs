using AuctionAPI.Contexts;
using AuctionAPI.Interfaces;
using AuctionAPI.Mappers;
using AuctionAPI.Misc;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionAPI.Tests.Misc
{
    [TestFixture]
    public class FunctionalitiesTests
    {
        private Functionalities _functionalities;
        private Mock<AuctionContext> _contextMock;
        private Mock<IValidation> _validationMock;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private Mock<IRepository<string, User>> _userRepoMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AuctionContext(options);

            _validationMock = new Mock<IValidation>();
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _userRepoMock = new Mock<IRepository<string, User>>();

            _functionalities = new Functionalities(
                context,
                _encryptionServiceMock.Object,
                _validationMock.Object,
                _userRepoMock.Object
            );
        }


        [Test]
        public async Task RegisterUser_Success()
        {
            // Arrange
            var userDto = new AddUserDto
            {
                Email = "test@example.com",
                Password = "pass123",
                PAN = "ABCDE1234F",
                Aadhar = "123456789012",
                Name = "Test"
            };

            _encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<EncryptModel>()))
                .ReturnsAsync((EncryptModel model) => new EncryptModel { EncryptedData = $"encrypted-{model.Data}" });

            // Act
            var result = await _functionalities.RegisterUser(userDto);

            // Assert
            Assert.That(result.Email, Is.EqualTo(userDto.Email));
            Assert.That(result.Password, Is.EqualTo("encrypted-pass123"));
            Assert.That(result.PAN, Is.EqualTo("encrypted-ABCDE1234F"));
            Assert.That(result.Aadhar, Is.EqualTo("encrypted-123456789012"));
        }


        [Test]
        public async Task GetUserDetails_Success()
        {
            // Arrange
            var email = "user@example.com";
            var user = new User { Email = email };

            _userRepoMock.Setup(r => r.Get(email)).ReturnsAsync(user);

            // Act
            var result = await _functionalities.GetUserDetails(email);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(email));
        }

        [Test]
        public void GetUserDetails_Exception()
        {
            // Arrange
            var email = "user@gmail.com";

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() =>
                _functionalities.GetUserDetails(email));
            Assert.That(ex.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public void GetUserDetails_Empty()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
                _functionalities.GetUserDetails("   "));
            Assert.That(ex.ParamName, Is.EqualTo("email"));
        }

    }
}
