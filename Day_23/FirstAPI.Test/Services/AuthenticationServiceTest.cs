using System;
using System.Threading.Tasks;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using FirstAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace FirstAPI.Test
{
    public class AuthenticationServiceTests
    {
        private Mock<ITokenService> _mockTokenService;
        private Mock<IRepository<string, User>> _mockUserRepository;
        private Mock<ILogger<AuthenticationService>> _mockLogger;

        private AuthenticationService _authService;

        [SetUp]
        public void Setup()
        {
            _mockTokenService = new Mock<ITokenService>();
            _mockUserRepository = new Mock<IRepository<string, User>>();
            _mockLogger = new Mock<ILogger<AuthenticationService>>();

            _authService = new AuthenticationService(
                _mockTokenService.Object,
                _mockUserRepository.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task Login_ReturnsToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new UserLoginRequest
            {
                Username = "test@gmail.com",
                Password = "qwerty"
            };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");

            var dbUser = new User
            {
                Username = "test@gmail.com",
                Password = hashedPassword,
                Role = "Patient"
            };

            _mockUserRepository.Setup(repo => repo.Get(loginRequest.Username))
                .ReturnsAsync(dbUser);

            _mockTokenService.Setup(token => token.GenerateToken(dbUser))
                .ReturnsAsync("token");

            // Act
            var result = await _authService.Login(loginRequest);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo("test@example.com"));
            Assert.That(result.Token, Is.EqualTo("token"));
        }

        [Test]
        public void Login_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            var loginRequest = new UserLoginRequest
            {
                Username = "test11@gmail.com.com",
                Password = "qwerty"
            };

            _mockUserRepository.Setup(repo => repo.Get(loginRequest.Username))
                .ReturnsAsync((User)null!); 

            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _authService.Login(loginRequest));
            //Assert
            Assert.That(ex!.Message, Is.EqualTo("No such user"));
        }

        [Test]
        public void Login_ThrowsException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginRequest = new UserLoginRequest
            {
                Username = "test@gmail.com",
                Password = "qwer"
            };

            var dbUser = new User
            {
                Username = "test@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("qwerty"),
                Role = "Patient"
            };

            _mockUserRepository.Setup(repo => repo.Get(loginRequest.Username))
                .ReturnsAsync(dbUser);

            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _authService.Login(loginRequest));
            // Assert
            Assert.That(ex!.Message, Is.EqualTo("Invalid password"));
        }
    }
}
