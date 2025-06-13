using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using AuctionAPI.Services;
using AuctionAPI.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;


namespace AuctionAPI.Tests.Services
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private Mock<ITokenService> _mockTokenService;
        private Mock<IRepository<string, User>> _mockUserRepository;
        private Mock<IRepository<string, RefreshToken>> _mockRefreshRepo;
        private Mock<ILogger<AuthenticationService>> _mockLogger;
        private AuthenticationService _authService;

        [SetUp]
        public void SetUp()
        {
            _mockTokenService = new Mock<ITokenService>();
            _mockUserRepository = new Mock<IRepository<string, User>>();
            _mockRefreshRepo = new Mock<IRepository<string, RefreshToken>>();
            _mockLogger = new Mock<ILogger<AuthenticationService>>();
            _authService = new AuthenticationService(
                _mockTokenService.Object,
                _mockUserRepository.Object,
                _mockRefreshRepo.Object,
                _mockLogger.Object);
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsUserLoginResponse()
        {
            var user = new User { Email = "test@example.com", Password = BCrypt.Net.BCrypt.HashPassword("password") };
            var request = new UserLoginRequest { Email = "test@example.com", Password = "password" };

            _mockUserRepository.Setup(r => r.Get("test@example.com")).ReturnsAsync(user);
            _mockTokenService.Setup(t => t.GenerateToken(user)).ReturnsAsync("token");
            _mockTokenService.Setup(t => t.GenerateRefreshToken()).ReturnsAsync("refresh_token");

            var result = await _authService.Login(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Token, Is.EqualTo("token"));
            Assert.That(result.RefreshToken, Is.EqualTo("refresh_token"));
        }

        [Test]
        public void Login_UserNotFound_ThrowsException()
        {
            var request = new UserLoginRequest { Email = "test@example.com", Password = "password" };

            _mockUserRepository.Setup(r => r.Get("test@example.com")).ReturnsAsync((User)null!);

            var ex = Assert.ThrowsAsync<CollectionEmptyException>(async () => await _authService.Login(request));
            Assert.That(ex.Message, Is.EqualTo("No such user"));
        }

        [Test]
        public void Login_InvalidPassword_ThrowsException()
        {
            var user = new User { Email = "test@example.com", Password = BCrypt.Net.BCrypt.HashPassword("wrongpass") };
            var request = new UserLoginRequest { Email = "test@example.com", Password = "password" };

            _mockUserRepository.Setup(r => r.Get("test@example.com")).ReturnsAsync(user);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _authService.Login(request));
            Assert.That(ex.Message, Is.EqualTo("Invalid password"));
        }

        [Test]
        public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokens()
        {
            var storedToken = new RefreshToken
            {
                Token = "old_token",
                UserEmail = "user@example.com",
                IsRevoked = false,
                Expires = DateTime.UtcNow.AddDays(1)
            };
            var user = new User { Email = "user@example.com" };

            _mockRefreshRepo.Setup(r => r.Get("old_token")).ReturnsAsync(storedToken);
            _mockUserRepository.Setup(r => r.Get("user@example.com")).ReturnsAsync(user);
            _mockTokenService.Setup(t => t.GenerateToken(user)).ReturnsAsync("new_token");
            _mockTokenService.Setup(t => t.GenerateRefreshToken()).ReturnsAsync("new_refresh");

            var result = await _authService.RefreshTokenAsync("old_token");

            Assert.That(result.AccessToken, Is.EqualTo("new_token"));
            Assert.That(result.RefreshToken, Is.EqualTo("new_refresh"));
        }

        [Test]
        public void RefreshTokenAsync_InvalidToken_ThrowsException()
        {
            _mockRefreshRepo.Setup(r => r.Get("invalid")).ReturnsAsync((RefreshToken)null!);

            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _authService.RefreshTokenAsync("invalid"));

            Assert.That(ex.Message, Is.EqualTo("Invalid refresh token"));
        }

        [Test]
        public async Task Logout_ValidToken_ReturnsSuccessMessage()
        {
            var token = new TokenRefreshRequest { RefreshToken = "token123" };
            var refreshToken = new RefreshToken { Token = "token123", IsRevoked = false };

            _mockRefreshRepo.Setup(r => r.Get("token123")).ReturnsAsync(refreshToken);
            _mockRefreshRepo.Setup(r => r.Update("token123", It.IsAny<RefreshToken>())).ReturnsAsync(refreshToken);

            var result = await _authService.Logout(token);

            Assert.That(result, Is.EqualTo("Logged Out Successfully"));
        }

        [Test]
        public void Logout_InvalidToken_ThrowsGenericException()
        {
            // Arrange
            var token = new TokenRefreshRequest { RefreshToken = "token123" };
            _mockRefreshRepo.Setup(r => r.Get("token123")).ReturnsAsync((RefreshToken)null!);

            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _authService.Logout(token));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Invalid refresh token"));
        }

    }

}
