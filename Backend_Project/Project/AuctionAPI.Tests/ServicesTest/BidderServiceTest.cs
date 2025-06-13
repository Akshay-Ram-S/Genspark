using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using AuctionAPI.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionAPI.Tests.Services
{
    [TestFixture]
    public class BidderServiceTests
    {
        private Mock<IRepository<string, User>> _userRepositoryMock;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private Mock<IRepository<Guid, Bidder>> _bidderRepositoryMock;
        private Mock<IRepository<Guid, Audit>> _auditRepositoryMock;
        private Mock<IFunctionalities> _functionalitiesMock;
        private BidderService _bidderService;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IRepository<string, User>>();
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _bidderRepositoryMock = new Mock<IRepository<Guid, Bidder>>();
            _auditRepositoryMock = new Mock<IRepository<Guid, Audit>>();
            _functionalitiesMock = new Mock<IFunctionalities>();

            _bidderService = new BidderService(
                _userRepositoryMock.Object,
                _encryptionServiceMock.Object,
                _bidderRepositoryMock.Object,
                _auditRepositoryMock.Object,
                _functionalitiesMock.Object);
        }

        [Test]
        public async Task AddUser_Success()
        {
            var userDto = new AddUserDto { Name = "John", Email = "john@example.com", Password = "pass", PAN = "ABC", Aadhar = "XYZ" };
            var user = new User { UserId = Guid.NewGuid(), Email = userDto.Email, Name = userDto.Name };
            var bidder = new Bidder { BidderId = Guid.NewGuid(), UserId = user.UserId, User = user };

            _functionalitiesMock.Setup(f => f.RegisterUser(userDto)).ReturnsAsync(user);
            _bidderRepositoryMock.Setup(r => r.Add(It.IsAny<Bidder>())).ReturnsAsync(bidder);

            var result = await _bidderService.AddUser(userDto);

            Assert.That(result.User.Email, Is.EqualTo(userDto.Email));
            Assert.That(result.User.Name, Is.EqualTo(userDto.Name));
        }

        [Test]
        public async Task GetUser_Success()
        {
            var id = Guid.NewGuid();
            var bidder = new Bidder { BidderId = id, User = new User { Name = "John" } };
            _bidderRepositoryMock.Setup(r => r.Get(id)).ReturnsAsync(bidder);

            var result = await _bidderService.GetUser(id);

            Assert.That(result.BidderId, Is.EqualTo(id));
            Assert.That(result.User.Name, Is.EqualTo("John"));
        }

        [Test]
        public async Task GetAllUsers_Success()
        {
            var bidders = new List<Bidder>
            {
                new Bidder { User = new User { Name = "Alice" } },
                new Bidder { User = new User { Name = "Bob" } }
            };

            _bidderRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(bidders);

            var result = await _bidderService.GetAllUsers(1, 10);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(2).Items);
        }

        [Test]
        public async Task DeleteUser_Success()
        {
            var userId = Guid.NewGuid();
            var bidder = new Bidder { UserId = userId, User = new User { Email = "john@example.com" } };
            var user = new User { UserId = userId, Email = "john@example.com" };

            _bidderRepositoryMock.Setup(r => r.Get(userId)).ReturnsAsync(bidder);
            _userRepositoryMock.Setup(r => r.Get(user.Email)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.Update(user.Email, It.IsAny<User>())).ReturnsAsync(user);

            var result = await _bidderService.DeleteUser(userId);

            Assert.That(result.UserId, Is.EqualTo(userId));
            _userRepositoryMock.Verify(r => r.Update(user.Email, It.Is<User>(u => u.Status == "Deleted")), Times.Once);
        }

        [Test]
        public async Task UpdateUser_Success()
        {
            var bidderId = Guid.NewGuid();
            var updateDto = new UpdateUserDto { Name = "Updated", Password = "newpass" };
            var user = new User { Email = "email@example.com", Name = "Old" };
            var bidder = new Bidder { UserId = bidderId, User = user };

            _bidderRepositoryMock.Setup(r => r.Get(bidderId)).ReturnsAsync(bidder);
            _userRepositoryMock.Setup(r => r.Get(user.Email)).ReturnsAsync(user);
            _encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<EncryptModel>()))
                .ReturnsAsync(new EncryptModel { EncryptedData = "encrypted" });

            var result = await _bidderService.UpdateUser(bidderId, updateDto);

            Assert.That(result.User.Name, Is.EqualTo("Updated"));
            Assert.That(user.Password, Is.EqualTo("encrypted"));
        }

        [Test]
        public async Task UpdateUser_NotSuccess()
        {
            var bidderId = Guid.NewGuid();
            var updateDto = new UpdateUserDto { Name = "Updated", Password = "newpass" };
            var user = new User { Email = "email@example.com", Name = "Old" };
            var bidder = new Bidder { UserId = bidderId, User = user };

            _bidderRepositoryMock.Setup(r => r.Get(bidderId)).ReturnsAsync(bidder);
            _userRepositoryMock.Setup(r => r.Get(user.Email)).ReturnsAsync(user);
            _encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<EncryptModel>()))
                .ReturnsAsync(new EncryptModel { EncryptedData = "encrypted" });

            var result = await _bidderService.UpdateUser(bidderId, updateDto);

            Assert.That(result.BidderId, Is.Not.EqualTo("bidderId"));
            Assert.That(result.User.Email, Is.Not.EqualTo("Email"));
        }

    }
}
