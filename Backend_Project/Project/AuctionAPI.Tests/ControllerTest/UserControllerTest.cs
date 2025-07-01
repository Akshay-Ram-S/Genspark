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
        public async Task CreateUser_UnauthenticatedUserWithInvalidEmail_ReturnsBadRequest()
        {
            var user = new AddUserDto
            {
                Email = "invalid-email",
                Name = "John",
                Aadhar = "123456789012",
                PAN = "ABCDE1234F",
                Role = "seller"
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            // Act
            var result = await _controller.CreateUser(user);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest!.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }


        [Test]
        public async Task CreateUser_AuthenticatedUser_ReturnsForbidden()
        {
            
            var user = new AddUserDto
            {
                Email = "test@example.com",
                Name = "John",
                Aadhar = "123456789012",
                PAN = "ABCDE1234F",
                Role = "seller"
            };

            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Email, "existing@example.com")
                    }, "mock")) // IsAuthenticated = true
                }
            };

            // Act
            var result = await _controller.CreateUser(user);

            // Assert
            var forbidden = result as ObjectResult;
            Assert.That(forbidden, Is.Not.Null);
            Assert.That(forbidden!.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
        }


        [Test]
        public async Task UpdateUser_Success()
        {
            var email = "user@test.com";
            var updateDto = new UpdateUserDto { NewPassword = "Updated" };
            var updatedUser = new User { Email = email };

            _mockUserService
                .Setup(s => s.UpdateUser(email, It.IsAny<UpdateUserDto>()))
                .ReturnsAsync(updatedUser);

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, email) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var result = await _controller.UpdateUser(Guid.NewGuid(), updateDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            _mockUserService.Verify(s => s.UpdateUser(email, It.IsAny<UpdateUserDto>()), Times.Once);
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
