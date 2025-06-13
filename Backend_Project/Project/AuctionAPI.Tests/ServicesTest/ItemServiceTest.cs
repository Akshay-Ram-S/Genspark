using AuctionAPI.Interfaces;
using AuctionAPI.Mappers;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using AuctionAPI.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionAPI.Tests
{
    [TestFixture]
    public class ItemServiceTests
    {
        private Mock<IRepository<Guid, Item>> _itemRepoMock = null!;
        private Mock<IRepository<Guid, ItemDetails>> _itemDetailsRepoMock = null!;
        private Mock<IRepository<Guid, Bidder>> _bidderRepoMock = null!;
        private Mock<IRepository<Guid, Audit>> _auditRepoMock = null!;
        private Mock<IRepository<Guid, Seller>> _sellerRepoMock = null!;
        private ItemService _itemService = null!;

        [SetUp]
        public void Setup()
        {
            _itemRepoMock = new Mock<IRepository<Guid, Item>>();
            _itemDetailsRepoMock = new Mock<IRepository<Guid, ItemDetails>>();
            _bidderRepoMock = new Mock<IRepository<Guid, Bidder>>();
            _auditRepoMock = new Mock<IRepository<Guid, Audit>>();
            _sellerRepoMock = new Mock<IRepository<Guid, Seller>>();

            _itemService = new ItemService(
                _itemRepoMock.Object,
                _sellerRepoMock.Object,
                _itemDetailsRepoMock.Object,
                _bidderRepoMock.Object,
                _auditRepoMock.Object
            );
        }

        [Test]
        public async Task CreateItem_Success()
        {
            var dto = new ItemCreateDto { Title = "Item", StartingPrice = 100, Description = "Test", SellerID = Guid.NewGuid() };
            var item = new Item { Id = Guid.NewGuid(), SellerID = dto.SellerID };
            var seller = new Seller { SellerId = dto.SellerID, User = new User { Email = "seller@gmail.com" } };

            _sellerRepoMock.Setup(x => x.Get(dto.SellerID)).ReturnsAsync(seller);
            _itemRepoMock.Setup(x => x.Add(It.IsAny<Item>())).ReturnsAsync(item);
            _itemDetailsRepoMock.Setup(x => x.Add(It.IsAny<ItemDetails>())).ReturnsAsync(new ItemDetails());
            _sellerRepoMock.Setup(x => x.Update(seller.SellerId, seller)).ReturnsAsync(seller);
            _auditRepoMock.Setup(x => x.Add(It.IsAny<Audit>())).ReturnsAsync(new Audit());

            var result = await _itemService.CreateItemAsync(dto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ItemID, Is.EqualTo(item.Id));
        }

        [Test]
        public void CreateItemAsync_Exception()
        {
            var dto = new ItemCreateDto { SellerID = Guid.NewGuid() };
            _sellerRepoMock.Setup(x => x.Get(dto.SellerID)).ReturnsAsync((Seller?)null);

            Assert.ThrowsAsync<Exception>(async () => await _itemService.CreateItemAsync(dto));
        }

        [Test]
        public async Task GetItemById_Success()
        {
            var id = Guid.NewGuid();
            var item = new Item { Id = id, SellerID = Guid.NewGuid() };
            var itemDetail = new ItemDetails { ItemId = id };
            var seller = new Seller { SellerId = item.SellerID, User = new User { Email = "user@gmail.com" } };

            _itemRepoMock.Setup(x => x.Get(id)).ReturnsAsync(item);
            _itemDetailsRepoMock.Setup(x => x.Get(id)).ReturnsAsync(itemDetail);
            _sellerRepoMock.Setup(x => x.Get(item.SellerID)).ReturnsAsync(seller);

            var result = await _itemService.GetItemById(id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ItemID, Is.EqualTo(id));
        }

        [Test]
        public void GetItemById_Exception()
        {
            var id = Guid.NewGuid();
            _itemRepoMock.Setup(x => x.Get(id)).ReturnsAsync((Item?)null);

            Assert.ThrowsAsync<Exception>(async () => await _itemService.GetItemById(id));
        }


        [Test]
        public async Task DeleteItem_Success()
        {
            var id = Guid.NewGuid();
            var item = new Item { Id = id, SellerID = Guid.NewGuid(), Seller = new Seller { User = new User { Email = "email" } } };
            var seller = new Seller { SellerId = item.SellerID, User = new User { Email = "email" } };

            _itemRepoMock.Setup(x => x.Get(id)).ReturnsAsync(item);
            _sellerRepoMock.Setup(x => x.Get(item.SellerID)).ReturnsAsync(seller);
            _itemRepoMock.Setup(x => x.Update(id, It.IsAny<Item>())).ReturnsAsync(item);
            _auditRepoMock.Setup(x => x.Add(It.IsAny<Audit>())).ReturnsAsync(new Audit());

            var result = await _itemService.DeleteItem(id, "email");
            Assert.That(result.IsDeleted, Is.True);
        }

        [Test]
        public async Task DeleteItem_Unauthorized()
        {
            var id = Guid.NewGuid();
            var item = new Item { Id = id, SellerID = Guid.NewGuid(), Seller = new Seller { User = new User { Email = "seller@gmail.com" } } };
            var seller = new Seller { SellerId = item.SellerID, User = new User { Email = "bidder@gmail.com" } };

            _itemRepoMock.Setup(x => x.Get(id)).ReturnsAsync(item);
            _sellerRepoMock.Setup(x => x.Get(item.SellerID)).ReturnsAsync(seller);

            var result = await _itemService.DeleteItem(id, "dummy@gmail.com");
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateItem_Success()
        {
            var id = Guid.NewGuid();
            var dto = new ItemUpdateDto { Title = "New", Description = "Desc" };
            var item = new Item { Id = id, SellerID = Guid.NewGuid(), Seller = new Seller { User = new User { Email = "user@gmail.com" } } };
            var itemDetails = new ItemDetails { ItemId = id };
            var seller = new Seller { SellerId = item.SellerID, User = new User { Email = "user@gmail.com" } };

            _itemRepoMock.Setup(x => x.Get(id)).ReturnsAsync(item);
            _sellerRepoMock.Setup(x => x.Get(item.SellerID)).ReturnsAsync(seller);
            _itemDetailsRepoMock.Setup(x => x.Get(id)).ReturnsAsync(itemDetails);
            _itemRepoMock.Setup(x => x.Update(id, item)).ReturnsAsync(item);
            _itemDetailsRepoMock.Setup(x => x.Update(id, itemDetails)).ReturnsAsync(itemDetails);

            var result = await _itemService.UpdateItem(id, dto, "user@gmail.com");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("New"));
        }

        [Test]
        public void UpdateItem_Exception()
        {
            var id = Guid.NewGuid();
            _itemRepoMock.Setup(x => x.Get(id)).ReturnsAsync((Item?)null);
            Assert.ThrowsAsync<Exception>(async () => await _itemService.UpdateItem(id, new ItemUpdateDto(), "email"));
        }

        [Test]
        public void UpdateItem_Exception2()
        {
            var id = Guid.NewGuid();
            var item = new Item { Id = id, SellerID = Guid.NewGuid(), Seller = new Seller { User = new User { Email = "test@gmail.com" } } };
            var seller = new Seller { SellerId = item.SellerID, User = new User { Email = "test@gmail.com" } };

            _itemRepoMock.Setup(x => x.Get(id)).ReturnsAsync(item);
            _sellerRepoMock.Setup(x => x.Get(item.SellerID)).ReturnsAsync(seller);
            _itemDetailsRepoMock.Setup(x => x.Get(id)).ReturnsAsync((ItemDetails?)null);

            Assert.ThrowsAsync<Exception>(async () => await _itemService.UpdateItem(id, new ItemUpdateDto(), "test@gmail.com"));
        }

        [Test]
        public void DeleteItem_ShouldThrowException_WhenItemNotFound()
        {
            var id = Guid.NewGuid();
            _itemRepoMock.Setup(x => x.Get(id)).ReturnsAsync((Item?)null);

            Assert.ThrowsAsync<Exception>(async () => await _itemService.DeleteItem(id, "email"));
        }
    }
}