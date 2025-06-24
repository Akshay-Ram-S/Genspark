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


    }
}
