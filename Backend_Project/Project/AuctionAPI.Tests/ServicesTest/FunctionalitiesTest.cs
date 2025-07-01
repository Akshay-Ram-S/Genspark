using AuctionAPI.Contexts;
using AuctionAPI.Interfaces;
using AuctionAPI.Misc;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace AuctionAPI.Tests.Misc
{
    [TestFixture]
    public class FunctionalitiesTests
    {
        private Functionalities _functionalities;
        private AuctionContext _context;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private Mock<IValidation> _validationMock;
        private Mock<IRepository<string, User>> _userRepoMock;
        private Mock<IRepository<Guid, Seller>> _sellerRepoMock;
        private Mock<IRepository<Guid, Bidder>> _bidderRepoMock;
        private Mock<IRepository<Guid, Item>> _itemRepoMock;
        private Mock<IRepository<Guid, ItemDetails>> _itemDetailsRepoMock;

        [SetUp]
        public void Setup()
        {
            // In-memory DB context setup
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AuctionContext(options);

            // Mock all dependencies
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _validationMock = new Mock<IValidation>();
            _userRepoMock = new Mock<IRepository<string, User>>();
            _sellerRepoMock = new Mock<IRepository<Guid, Seller>>();
            _bidderRepoMock = new Mock<IRepository<Guid, Bidder>>();
            _itemRepoMock = new Mock<IRepository<Guid, Item>>();
            _itemDetailsRepoMock = new Mock<IRepository<Guid, ItemDetails>>();

            // Inject all dependencies into Functionalities
            _functionalities = new Functionalities(
                _context,
                _encryptionServiceMock.Object,
                _validationMock.Object,
                _userRepoMock.Object,
                _sellerRepoMock.Object,
                _bidderRepoMock.Object,
                _itemRepoMock.Object,
                _itemDetailsRepoMock.Object
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
                .ReturnsAsync((EncryptModel model) => new EncryptModel
                {
                    EncryptedData = $"encrypted-{model.Data}"
                });

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
        public void GetUserDetails_UserNotFound_ThrowsException()
        {
            // Arrange
            var email = "nonexistent@example.com";

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _functionalities.GetUserDetails(email));
            Assert.That(ex!.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public void GetUserDetails_EmptyEmail_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _functionalities.GetUserDetails("   "));
            Assert.That(ex!.ParamName, Is.EqualTo("email"));
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

    }
    
}
