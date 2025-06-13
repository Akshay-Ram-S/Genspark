using System.Security.Claims;
using AuctionAPI.Controllers;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionAPI.Tests.Controllers
{
    [TestFixture]
    public class ItemsControllerTests
    {
        private Mock<IItemService> _itemServiceMock;
        private Mock<IFunctionalities> _functionalitiesMock;
        private Mock<ILogger<ItemsController>> _loggerMock;
        private ItemsController _controller;

        [SetUp]
        public void SetUp()
        {
            _itemServiceMock = new Mock<IItemService>();
            _functionalitiesMock = new Mock<IFunctionalities>();
            _loggerMock = new Mock<ILogger<ItemsController>>();

            _controller = new ItemsController(_loggerMock.Object, _itemServiceMock.Object, _functionalitiesMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Role, "Seller")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task CreateItem_Test()
        {
            var dto = new ItemCreateDto { EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)) };
            var createdItem = new ItemResponse
            {
                ItemID = Guid.NewGuid(),
                Title = "Test Item",
                Status = "Active"
            };

            _itemServiceMock.Setup(s => s.CreateItemAsync(dto)).ReturnsAsync(createdItem);

            var result = await _controller.CreateItem(dto) as CreatedAtActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));
        }

        [Test]
        public async Task UpdateItem_Exception()
        {
            var id = Guid.NewGuid();
            var dto = new ItemUpdateDto { EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)) };
            var updatedItem = new ItemResponse
            {
                ItemID = id,
                Title = "Updated Item",
                Status = "Active"
            };

            _itemServiceMock.Setup(s => s.UpdateItem(id, dto, "test@example.com")).ReturnsAsync(updatedItem);

            var result = await _controller.UpdateItemById(id, dto) as OkObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetItem_Test()
        {
            var itemId = Guid.NewGuid();
            var itemResponse = new ItemResponse
            {
                ItemID = itemId,
                Title = "Laptop",
                Status = "Active"
            };

            _itemServiceMock.Setup(s => s.GetItemById(itemId)).ReturnsAsync(itemResponse);

            var result = await _controller.GetItem(itemId) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }


        [Test]

        public async Task GetAllBids_Test()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var bids = new List<ItemAllBids>
            {
                new ItemAllBids
                {
                    title = "Laptop",
                    bidder_id = Guid.NewGuid(),
                    name = "John Doe",
                    amount = 1500.00m,
                    bid_timestamp = DateTime.UtcNow
                }
            };

            _functionalitiesMock.Setup(f => f.AllBids(itemId)).ReturnsAsync(bids);

            // Act
            var result = await _controller.GetAllBids(itemId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            
        }


        [Test]
        public async Task DeleteItem_Test()
        {
            var id = Guid.NewGuid();
            var item = new Item { Id = id };

            _itemServiceMock.Setup(s => s.DeleteItem(id, "test@example.com")).ReturnsAsync(item);

            var result = await _controller.DeleteItemById(id) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }
    }
}
