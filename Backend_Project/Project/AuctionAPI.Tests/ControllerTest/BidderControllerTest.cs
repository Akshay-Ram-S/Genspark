using AuctionAPI.Controllers;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionAPI.Tests.Controllers
{
    public class BidderControllerTests
    {
        private Mock<IUserService<Bidder>> _mockBidderService;
        private Mock<ILogger<BidderController>> _mockLogger;
        private Mock<IValidation> _mockValidation;
        private Mock<IFunctionalities> _mockFunctionalities;
        private BidderController _controller;

        [SetUp]
        public void Setup()
        {
            _mockBidderService = new Mock<IUserService<Bidder>>();
            _mockLogger = new Mock<ILogger<BidderController>>();
            _mockValidation = new Mock<IValidation>();
            _mockFunctionalities = new Mock<IFunctionalities>();

            _controller = new BidderController(
                _mockBidderService.Object,
                _mockLogger.Object,
                _mockValidation.Object,
                _mockFunctionalities.Object
            );
        }

        [Test]
        public async Task GetBidderById_ReturnsBidder_WhenExists()
        {
            var id = Guid.NewGuid();
            var bidder = new Bidder { BidderId = id, User = new User { Name = "Ravi" } };
            _mockBidderService.Setup(s => s.GetUser(id)).ReturnsAsync(bidder);

            var result = await _controller.GetBidderById(id) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetBidderById_ReturnsNotFound_WhenNull()
        {
            var id = Guid.NewGuid();
            _mockBidderService.Setup(s => s.GetUser(id)).ReturnsAsync((Bidder)null!);

            var result = await _controller.GetBidderById(id);

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetBidderById_ThrowsException_Returns500()
        {
            var id = Guid.NewGuid();
            _mockBidderService.Setup(s => s.GetUser(id)).ThrowsAsync(new Exception("Error"));

            var result = await _controller.GetBidderById(id) as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetBidders_ReturnsPagedBidders()
        {
            _mockBidderService.Setup(s => s.GetAllUsers(1, 10))
                .ReturnsAsync(new List<Bidder> { new Bidder { User = new User { Name = "Test" } } });

            var result = await _controller.GetBidders(1, 10) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetBidders_ThrowsException_Returns500()
        {
            _mockBidderService.Setup(s => s.GetAllUsers(1, 10))
                .ThrowsAsync(new Exception("Fetch failed"));

            var result = await _controller.GetBidders(1, 10) as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task DeleteBidder_ReturnsDeletedBidder_WhenExists()
        {
            var id = Guid.NewGuid();
            var bidder = new Bidder { BidderId = id, User = new User { Email = "a@test.com" } };
            _mockBidderService.Setup(s => s.DeleteUser(id)).ReturnsAsync(bidder);

            var result = await _controller.DeleteBidder(id) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task DeleteBidder_ReturnsNotFound_WhenBidderNotFound()
        {
            var id = Guid.NewGuid();
            _mockBidderService.Setup(s => s.DeleteUser(id)).ReturnsAsync((Bidder)null!);

            var result = await _controller.DeleteBidder(id);

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteBidder_ThrowsException_Returns500()
        {
            var id = Guid.NewGuid();
            _mockBidderService.Setup(s => s.DeleteUser(id)).ThrowsAsync(new Exception("Error"));

            var result = await _controller.DeleteBidder(id) as ObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(500));
        }
    }
}