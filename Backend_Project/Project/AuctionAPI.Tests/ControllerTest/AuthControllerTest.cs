using AuctionAPI.Controllers;
using AuctionAPI.Interfaces;
using AuctionAPI.Models.DTOs;
using AuctionAPI.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuctionAPI.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthenticationService> _mockAuthService;
        private Mock<ILogger<AuthController>> _mockLogger;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task UserLogin_Success()
        {
            var loginRequest = new UserLoginRequest { Email = "test@example.com", Password = "password" };
            var loginResponse = new UserLoginResponse { Email = loginRequest.Email, Token = "jwt", RefreshToken = "refresh" };

            _mockAuthService.Setup(a => a.Login(loginRequest)).ReturnsAsync(loginResponse);

            var result = await _controller.UserLogin(loginRequest) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task UserLogin_Exception()
        {
            var loginRequest = new UserLoginRequest { Email = "wrong@example.com", Password = "wrong" };

            _mockAuthService.Setup(a => a.Login(loginRequest)).ThrowsAsync(new Exception("Invalid login"));

            var result = await _controller.UserLogin(loginRequest) as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(401));
        }


        [Test]
        public async Task Refresh_Exception()
        {
            var request = new TokenRefreshRequest { RefreshToken = "invalid" };

            _mockAuthService.Setup(a => a.RefreshTokenAsync(request.RefreshToken!))
                            .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.Refresh(request) as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(401));
        }

        [Test]
        public async Task Logout_BadRequest()
        {
            var result = await _controller.UserLogout(new TokenRefreshRequest { RefreshToken = null }) as BadRequestObjectResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task Logout_Success()
        {
            var token = new TokenRefreshRequest { RefreshToken = "valid_token" };
            _mockAuthService.Setup(a => a.Logout(token)).ReturnsAsync("Logged out");

            var result = await _controller.UserLogout(token) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void MyProfile_Success()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-id"),
                new Claim(ClaimTypes.Email, "user@example.com"),
                new Claim(ClaimTypes.Role, "Bidder")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var result = _controller.MyProfile() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
        }
    }
}
