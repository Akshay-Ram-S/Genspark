using AuctionAPI.Controllers;
using AuctionAPI.Models.DTOs;
using AuctionAPI.Interfaces;
using AuctionAPI.Mappers;
using AuctionAPI.Misc;
using AuctionAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Security.Claims;

namespace AuctionAPI.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService<Seller>> _mockSellerService;
        private Mock<IUserService<Bidder>> _mockBidderService;
        private Mock<ICommonUserService> _mockUserService;
        private Mock<ILogger<SellerController>> _mockLogger;
        private Mock<IValidation> _mockValidation;
        private Mock<IFunctionalities> _mockFunctionalities;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _mockSellerService = new Mock<IUserService<Seller>>();
            _mockBidderService = new Mock<IUserService<Bidder>>();
            _mockUserService = new Mock<ICommonUserService>();
            _mockLogger = new Mock<ILogger<SellerController>>();
            _mockValidation = new Mock<IValidation>();
            _mockFunctionalities = new Mock<IFunctionalities>();

            _controller = new UserController(
                _mockSellerService.Object,
                _mockBidderService.Object,
                _mockUserService.Object,
                _mockLogger.Object,
                _mockValidation.Object,
                _mockFunctionalities.Object
            );
        }

        [Test]
        public async Task CreateUser_Success()
        {
            var user = new AddUserDto
            {
                Email = "user@gmail.com",
                Name = "John",
                Aadhar = "123456789012",
                PAN = "ABCDE1234F"
            };
            var seller = new Seller { SellerId = Guid.NewGuid()};

            _mockValidation.Setup(v => v.IsValidEmail(user.Email)).Returns(true);
            _mockValidation.Setup(v => v.ValidName(user.Name)).Returns(true);
            _mockValidation.Setup(v => v.ValidAadharAndPAN(user.Aadhar, user.PAN.ToUpper())).ReturnsAsync(true);
            _mockValidation.Setup(v => v.EmailExists(user.Email)).ReturnsAsync(false);
            _mockSellerService.Setup(s => s.AddUser(user)).ReturnsAsync(seller);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.CreateUser(user) as CreatedAtActionResult;

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateUser_Failure()
        {
            var user = new AddUserDto { Email = "invalid@role.com", Name = "Test", Aadhar = "123", PAN = "ABCDE1234F" };

            _mockValidation.Setup(v => v.IsValidEmail(It.IsAny<string>())).Returns(true);
            _mockValidation.Setup(v => v.ValidName(It.IsAny<string>())).Returns(true);
            _mockValidation.Setup(v => v.ValidAadharAndPAN(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            _mockValidation.Setup(v => v.EmailExists(It.IsAny<string>())).ReturnsAsync(false);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.CreateUser(user) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task UpdateUser_Success()
        {
            var email = "user@test.com";
            var updateDto = new UpdateUserDto { Name = "Updated Name" };
            var updatedUser = new User { Email = email };

            _mockUserService.Setup(s => s.UpdateUser(email, updateDto)).ReturnsAsync(updatedUser);

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, email) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var result = await _controller.UpdateUser(Guid.NewGuid(), updateDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task DeleteUser_Success()
        {
            var email = "delete@gmail.com";
            var deletedUser = new User { Email = email };

            _mockUserService.Setup(s => s.DeleteUser(email)).ReturnsAsync(deletedUser);

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, email) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var result = await _controller.DeleteUser() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }
    }
}
