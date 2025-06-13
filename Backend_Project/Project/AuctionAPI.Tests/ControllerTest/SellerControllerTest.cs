using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using AuctionAPI.Controllers;
using AuctionAPI.Models;
using AuctionAPI.Interfaces;
using AuctionAPI.Models.DTOs;
using AuctionAPI.Mappers;
using Microsoft.AspNetCore.Http;

namespace AuctionAPI.Tests.Controllers
{
    [TestFixture]
    public class SellerControllerTests
    {
        private Mock<IUserService<Seller>> _mockSellerService;
        private Mock<IUserService<Bidder>> _mockBidderService;
        private Mock<ILogger<SellerController>> _mockLogger;
        private Mock<IValidation> _mockValidation;
        private Mock<IFunctionalities> _mockFunctionalities;
        private SellerController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockSellerService = new Mock<IUserService<Seller>>();
            _mockBidderService = new Mock<IUserService<Bidder>>();
            _mockLogger = new Mock<ILogger<SellerController>>();
            _mockValidation = new Mock<IValidation>();
            _mockFunctionalities = new Mock<IFunctionalities>();

            _controller = new SellerController(
                _mockSellerService.Object,
                _mockBidderService.Object,
                _mockLogger.Object,
                _mockValidation.Object,
                _mockFunctionalities.Object
            );
        }

        
        [Test]
        public async Task GetSellers_Success()
        {
            var sellers = new List<Seller> { new Seller { SellerId = Guid.NewGuid(), User = new User { Name = "Test" } } };
            _mockSellerService.Setup(s => s.GetAllUsers(1, 10)).ReturnsAsync(sellers);

            var result = await _controller.GetSellers(1, 10) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetSellers_Exception()
        {
            _mockSellerService.Setup(s => s.GetAllUsers(It.IsAny<int>(), It.IsAny<int>()))
                              .ThrowsAsync(new Exception("Error"));

            var result = await _controller.GetSellers() as ObjectResult;

            Assert.That(result?.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetSellerById_Success()
        {
            var id = Guid.NewGuid();
            var seller = new Seller { SellerId = id, User = new User { Name = "Seller1" } };

            _mockSellerService.Setup(s => s.GetUser(id)).ReturnsAsync(seller);

            var result = await _controller.GetSellerById(id) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetSellerById_NotFound()
        {
            var id = Guid.NewGuid();
            _mockSellerService.Setup(s => s.GetUser(id)).ReturnsAsync((Seller)null);

            var result = await _controller.GetSellerById(id) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

    }
}