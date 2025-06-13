using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AuctionAPI.Contexts;
using AuctionAPI.Controllers;
using AuctionAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace AuctionAPI.Tests.Controllers
{
    [TestFixture]
    public class ImageControllerTests
    {
        private AuctionContext _context;
        private Mock<ILogger<ImageController>> _loggerMock;
        private ImageController _controller;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AuctionContext(options);
            _loggerMock = new Mock<ILogger<ImageController>>();
            _controller = new ImageController(_loggerMock.Object, _context);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task ViewImage_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            var result = await _controller.ViewImage(Guid.NewGuid()) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value!.ToString(), Is.EqualTo("AuctionAPI.Models.DTOs.ApiResponse`1[System.String]"));
        }


        [Test]
        public async Task ViewImage_ShouldReturnFile_WhenImageExists()
        {
            var itemId = Guid.NewGuid();
            var imageData = new byte[] { 1, 2, 3 };
            var mimeType = "image/png";

            await _context.ItemDetails.AddAsync(new ItemDetails
            {
                ItemId = itemId,
                ImageData = imageData,
                ImageMimeType = mimeType
            });
            await _context.SaveChangesAsync();

            var result = await _controller.ViewImage(itemId) as FileContentResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ContentType, Is.EqualTo(mimeType));
            Assert.That(result.FileContents, Is.EqualTo(imageData));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
