using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AuctionAPI.Controllers;
using AuctionAPI.Interfaces;
using AuctionAPI.Models.DTOs;
using AuctionAPI.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace AuctionAPI.Tests.Controllers
{
    [TestFixture]
    public class BidControllerTests
    {
        private Mock<IBidService> _mockBidService;
        private Mock<ILogger<BidController>> _mockLogger;
        private BidController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockBidService = new Mock<IBidService>();
            _mockLogger = new Mock<ILogger<BidController>>();
            _controller = new BidController(_mockLogger.Object, _mockBidService.Object);
        }

        [Test]
        public async Task PostBid_ValidBid()
        {
            // Arrange
            var bidDto = new BidCreateDTO
            {
                ItemId = Guid.NewGuid(),
                BidderId = Guid.NewGuid(),
                Amount = 150.0m
            };

            var expectedResponse = new BidResponse
            {
                Id = Guid.NewGuid(),
                ItemID = bidDto.ItemId,
                Amount = bidDto.Amount,
                BidderName = "John",
                Title = "Phone",
                Status = "Active",
                Timestamp = DateTime.UtcNow
            };

            _mockBidService.Setup(x => x.PlaceBid(bidDto)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.PostBid(bidDto);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult!.ActionName, Is.EqualTo(nameof(_controller.GetBid)));
            Assert.That(createdResult.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        }

        [Test]
        public async Task PostBid_Exception()
        {
            var bidDto = new BidCreateDTO { ItemId = Guid.NewGuid(), BidderId = Guid.NewGuid(), Amount = 100 };
            _mockBidService.Setup(x => x.PlaceBid(bidDto)).ThrowsAsync(new Exception("Invalid bid"));

            var result = await _controller.PostBid(bidDto);

            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest!.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task GetBid_Success()
        {
            var bidId = Guid.NewGuid();
            var expectedBid = new BidResponse
            {
                Id = bidId,
                Title = "Watch",
                ItemID = Guid.NewGuid(),
                Amount = 500,
                BidderName = "Alice",
                Status = "Active",
                Timestamp = DateTime.UtcNow
            };

            _mockBidService.Setup(x => x.GetBidById(bidId)).ReturnsAsync(expectedBid);

            var result = await _controller.GetBid(bidId);

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task GetBid_NotFound()
        {
            var bidId = Guid.NewGuid();
            _mockBidService.Setup(x => x.GetBidById(bidId)).ReturnsAsync((BidResponse)null!);

            var result = await _controller.GetBid(bidId);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task CancelBid_Success()
        {
            var bidId = Guid.NewGuid();
            var email = "user@example.com";

            var bidResponse = new BidResponse
            {
                Id = bidId,
                ItemID = Guid.NewGuid(),
                Title = "Tablet",
                BidderName = "Test",
                Amount = 200,
                Status = "Cancelled",
                Timestamp = DateTime.UtcNow
            };

            _mockBidService.Setup(x => x.CancelBid(bidId, email)).ReturnsAsync(bidResponse);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.CancelBid(bidId);

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task CancelBid_MissingClaim()
        {
            var bidId = Guid.NewGuid();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.CancelBid(bidId);

            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.That(unauthorizedResult, Is.Not.Null);
            Assert.That(unauthorizedResult!.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
        }

        [Test]
        public async Task CancelBid_Unauthorized()
        {
            var bidId = Guid.NewGuid();
            var email = "unauthorized@example.com";

            _mockBidService.Setup(x => x.CancelBid(bidId, email)).ReturnsAsync((BidResponse)null!);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.CancelBid(bidId);

            var forbiddenResult = result as ObjectResult;
            Assert.That(forbiddenResult, Is.Not.Null);
            Assert.That(forbiddenResult!.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
        }

        [Test]
        public async Task CancelBid_Exception()
        {
            var bidId = Guid.NewGuid();
            var email = "error@example.com";

            _mockBidService.Setup(x => x.CancelBid(bidId, email)).ThrowsAsync(new Exception("DB failure"));

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.CancelBid(bidId);

            var errorResult = result as ObjectResult;
            Assert.That(errorResult, Is.Not.Null);
            Assert.That(errorResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }
    }
}
