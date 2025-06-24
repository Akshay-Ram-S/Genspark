using NUnit.Framework;
using Moq;
using AuctionAPI.Controllers;
using AuctionAPI.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using AuctionAPI.Models.DTOs;
using System.Threading.Tasks;
using AuctionAPI.Mappers;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthenticationService> _mockAuthService;
    private Mock<ILogger<AuthController>> _mockLogger;
    private Mock<IFunctionalities> _mockFunctionalities;
    private AuthController _controller;

    [SetUp]
    public void Setup()
    {
        _mockAuthService = new Mock<IAuthenticationService>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _mockFunctionalities = new Mock<IFunctionalities>();

        _controller = new AuthController(
            _mockAuthService.Object,
            _mockLogger.Object,
            _mockFunctionalities.Object
        );
    }

        [Test]
    public async Task UserLogin_ValidRequest_ReturnsOk()
    {
        var request = new UserLoginRequest { Email = "test@example.com", Password = "pass" };
        var expectedToken = "jwt-token";

        var result = await _controller.UserLogin(request) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task UserLogin_InvalidCredentials_ReturnsUnauthorized()
    {
        var request = new UserLoginRequest { Email = "test@example.com", Password = "wrong" };
        _mockAuthService.Setup(s => s.Login(request)).ThrowsAsync(new Exception("Invalid"));

        var result = await _controller.UserLogin(request) as UnauthorizedObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(401));
    }

        [Test]
    public async Task Refresh_ValidToken_ReturnsNewJwt()
    {
        var refreshToken = "valid-token";

        var result = await _controller.Refresh(new TokenRefreshRequest { RefreshToken = refreshToken }) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task Refresh_InvalidToken_ReturnsUnauthorized()
    {
        _mockAuthService.Setup(s => s.RefreshTokenAsync(It.IsAny<string>())).ThrowsAsync(new UnauthorizedAccessException());

        var result = await _controller.Refresh(new TokenRefreshRequest { RefreshToken = "invalid" }) as UnauthorizedObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(401));
    }

        [Test]
    public async Task Logout_ValidRequest_ReturnsOk()
    {
        var token = new TokenRefreshRequest { RefreshToken = "token" };

        var result = await _controller.UserLogout(token) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task Logout_NullToken_ReturnsBadRequest()
    {
        var result = await _controller.UserLogout(null!) as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task Logout_EmptyToken_ReturnsBadRequest()
    {
        var token = new TokenRefreshRequest { RefreshToken = "" };

        var result = await _controller.UserLogout(token) as BadRequestObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(400));
    }

}

