using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using AuctionAPI.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionAPI.Tests
{
    [TestFixture]
    public class SellerServiceTests
    {
        private Mock<IRepository<string, User>> _mockUserRepo;
        private Mock<IEncryptionService> _mockEncryptionService;
        private Mock<IRepository<Guid, Seller>> _mockSellerRepo;
        private Mock<IRepository<Guid, Audit>> _mockAuditRepo;
        private Mock<IFunctionalities> _mockFunctionalities;
        private SellerService _sellerService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepo = new Mock<IRepository<string, User>>();
            _mockEncryptionService = new Mock<IEncryptionService>();
            _mockSellerRepo = new Mock<IRepository<Guid, Seller>>();
            _mockAuditRepo = new Mock<IRepository<Guid, Audit>>();
            _mockFunctionalities = new Mock<IFunctionalities>();

            _sellerService = new SellerService(
                _mockUserRepo.Object,
                _mockEncryptionService.Object,
                _mockSellerRepo.Object,
                _mockAuditRepo.Object,
                _mockFunctionalities.Object
            );
        }

        [Test]
        public async Task AddUser_Success()
        {
            var dto = new AddUserDto { Name = "Ramu", Email = "ramu@gmail.com", Password = "12345" };
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                Role = "Seller"
            };
            var seller = new Seller { SellerId = Guid.NewGuid(), UserId = user.UserId, User = user };

            _mockFunctionalities.Setup(f => f.RegisterUser(It.IsAny<AddUserDto>())).ReturnsAsync(user);
            _mockUserRepo.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(user);
            _mockSellerRepo.Setup(r => r.Add(It.IsAny<Seller>())).ReturnsAsync(seller);
            _mockAuditRepo.Setup(r => r.Add(It.IsAny<Audit>())).ReturnsAsync(new Audit());

            var result = await _sellerService.AddUser(dto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.User.Role, Is.EqualTo("Seller"));
        }

        [Test]
        public async Task GetUser_WhenFound()
        {
            var id = Guid.NewGuid();
            var seller = new Seller { SellerId = id, User = new User { Name = "Test" } };

            _mockSellerRepo.Setup(r => r.Get(id)).ReturnsAsync(seller);

            var result = await _sellerService.GetUser(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SellerId, Is.EqualTo(id));
        }

        [Test]
        public void GetUser_Exception()
        {
            var id = Guid.NewGuid();
            _mockSellerRepo.Setup(r => r.Get(id)).ReturnsAsync((Seller)null!);

            var ex = Assert.ThrowsAsync<Exception>(() => _sellerService.GetUser(id));
            Assert.That(ex!.Message, Is.EqualTo($"No seller found with id: {id}"));
        }

        [Test]
        public async Task GetAllUsers_ShouldReturnOrderedUsers_WhenAvailable()
        {
            var sellers = new List<Seller>
            {
                new Seller { User = new User { Name = "Zeta" } },
                new Seller { User = new User { Name = "Alpha" } },
                new Seller { User = new User { Name = "Beta" } }
            };

            _mockSellerRepo.Setup(r => r.GetAll()).ReturnsAsync(sellers);

            var result = (await _sellerService.GetAllUsers(1, 2)).ToList();

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].User.Name, Is.EqualTo("Alpha")); 
        }

        [Test]
        public void GetAllUsers_ShouldThrow_WhenNoneExist()
        {
            _mockSellerRepo.Setup(r => r.GetAll()).ReturnsAsync((IEnumerable<Seller>)null!);

            var ex = Assert.ThrowsAsync<Exception>(() => _sellerService.GetAllUsers(1, 10));
            Assert.That(ex!.Message, Is.EqualTo("No sellers found in the databse"));
        }

        [Test]
        public async Task DeleteUser_ShouldMarkUserDeleted_WhenFound()
        {
            var id = Guid.NewGuid();
            var user = new User { Email = "ramu@gmail.com", UserId = id, Status = "Active" };
            var seller = new Seller { SellerId = id, User = user };

            _mockSellerRepo.Setup(r => r.Get(id)).ReturnsAsync(seller);
            _mockUserRepo.Setup(r => r.Get(user.Email)).ReturnsAsync(user);
            _mockUserRepo.Setup(r => r.Update(user.Email, It.IsAny<User>())).ReturnsAsync(user);
            _mockAuditRepo.Setup(r => r.Add(It.IsAny<Audit>())).ReturnsAsync(new Audit());

            var result = await _sellerService.DeleteUser(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(user.Status, Is.EqualTo("Deleted"));
        }

        [Test]
        public void DeleteUser_ShouldThrow_WhenSellerNotFound()
        {
            var id = Guid.NewGuid();
            _mockSellerRepo.Setup(r => r.Get(id)).ReturnsAsync((Seller)null!);

            var ex = Assert.ThrowsAsync<Exception>(() => _sellerService.DeleteUser(id));
            Assert.That(ex!.Message, Is.EqualTo($"No seller found with id: {id}"));
        }

        [Test]
        public async Task UpdateUser_ShouldUpdateNameAndPassword_WhenValid()
        {
            var id = Guid.NewGuid();
            var email = "ramu@gmail.com";
            var seller = new Seller { SellerId = id, User = new User { Email = email } };
            var user = new User { Email = email };
            var dto = new UpdateUserDto { Name = "Updated", Password = "pass" };

            _mockSellerRepo.Setup(r => r.Get(id)).ReturnsAsync(seller);
            _mockUserRepo.Setup(r => r.Get(email)).ReturnsAsync(user);
            _mockEncryptionService.Setup(e => e.EncryptData(It.IsAny<EncryptModel>()))
                .ReturnsAsync(new EncryptModel { EncryptedData = "encrypted-pass" });
            _mockUserRepo.Setup(r => r.Update(email, user)).ReturnsAsync(user);
            _mockAuditRepo.Setup(r => r.Add(It.IsAny<Audit>())).ReturnsAsync(new Audit());

            var result = await _sellerService.UpdateUser(id, dto);

            Assert.That(result, Is.Not.Null);
            Assert.That(user.Name, Is.EqualTo(dto.Name));
            Assert.That(user.Password, Is.EqualTo("encrypted-pass"));
        }

        [Test]
        public void UpdateUser_ShouldThrow_WhenSellerNotFound()
        {
            var id = Guid.NewGuid();
            _mockSellerRepo.Setup(r => r.Get(id)).ReturnsAsync((Seller)null!);

            var ex = Assert.ThrowsAsync<Exception>(() => _sellerService.UpdateUser(id, new UpdateUserDto()));
            Assert.That(ex!.Message, Is.EqualTo($"No Seller found with id: {id}"));
        }

        [Test]
        public void UpdateUser_ShouldThrow_WhenUserNotFound()
        {
            var id = Guid.NewGuid();
            var email = "test@gmail.com";
            var seller = new Seller { SellerId = id, User = new User { Email = email } };

            _mockSellerRepo.Setup(r => r.Get(id)).ReturnsAsync(seller);
            _mockUserRepo.Setup(r => r.Get(email)).ReturnsAsync((User)null!);

            var ex = Assert.ThrowsAsync<Exception>(() => _sellerService.UpdateUser(id, new UpdateUserDto()));
        }
    }
}
