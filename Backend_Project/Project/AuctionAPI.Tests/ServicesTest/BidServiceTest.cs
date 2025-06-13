using AuctionAPI.Hubs;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using AuctionAPI.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionAPI.Tests.Services
{
    public class BidServiceTests
    {
        private Mock<IRepository<Guid, Bid>> _bidRepoMock = null!;
        private Mock<IRepository<Guid, Bidder>> _bidderRepoMock = null!;
        private Mock<IRepository<Guid, Item>> _itemRepoMock = null!;
        private Mock<IRepository<Guid, ItemDetails>> _itemDetailsRepoMock = null!;
        private Mock<IHubContext<AuctionHub>> _hubContextMock = null!;
        private Mock<IClientProxy> _clientProxyMock = null!;
        private BidService _service = null!;

        [SetUp]
        public void Setup()
        {
            _bidRepoMock = new Mock<IRepository<Guid, Bid>>();
            _bidderRepoMock = new Mock<IRepository<Guid, Bidder>>();
            _itemRepoMock = new Mock<IRepository<Guid, Item>>();
            _itemDetailsRepoMock = new Mock<IRepository<Guid, ItemDetails>>();
            _hubContextMock = new Mock<IHubContext<AuctionHub>>();
            _clientProxyMock = new Mock<IClientProxy>();

            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(c => c.All).Returns(_clientProxyMock.Object);
            _hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);

            _service = new BidService(
                _bidRepoMock.Object,
                _bidderRepoMock.Object,
                _itemRepoMock.Object,
                _itemDetailsRepoMock.Object,
                _hubContextMock.Object
            );
        }

        [Test]
        public async Task PlaceBid_ShouldReturnResponse_WhenValid()
        {
            var itemId = Guid.NewGuid();
            var bidderId = Guid.NewGuid();
            var bidDto = new BidCreateDTO { ItemId = itemId, BidderId = bidderId, Amount = 1000 };
            var item = new Item { Id = itemId, Title = "Test Item" };
            var user = new User { Name = "Test User" };
            var bidder = new Bidder { BidderId = bidderId, User = user, Bids = new List<Bid>() };
            var itemDetails = new ItemDetails { ItemId = itemId, StartingPrice = 500, CurrentBid = 800 };

            _itemRepoMock.Setup(r => r.Get(itemId)).ReturnsAsync(item);
            _bidderRepoMock.Setup(r => r.Get(bidderId)).ReturnsAsync(bidder);
            _itemDetailsRepoMock.Setup(r => r.Get(itemId)).ReturnsAsync(itemDetails);
            _bidRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Bid>());
            _bidRepoMock.Setup(r => r.Add(It.IsAny<Bid>())).ReturnsAsync((Bid b) => b);

            var result = await _service.PlaceBid(bidDto);

            Assert.That(result.Amount, Is.EqualTo(1000));
            Assert.That(result.Title, Is.EqualTo(item.Title));
        }


        [Test]
        public async Task GetBidById_ShouldReturnBidResponse_WhenBidExists()
        {
            var bidId = Guid.NewGuid();
            var bid = new Bid { Id = bidId, BidderId = Guid.NewGuid(), ItemId = Guid.NewGuid(), Amount = 1000 };
            var bidder = new Bidder { BidderId = bid.BidderId, User = new User { Name = "User" } };
            var item = new Item { Id = bid.ItemId, Title = "Item" };

            _bidRepoMock.Setup(r => r.Get(bidId)).ReturnsAsync(bid);
            _bidderRepoMock.Setup(r => r.Get(bid.BidderId)).ReturnsAsync(bidder);
            _itemRepoMock.Setup(r => r.Get(bid.ItemId)).ReturnsAsync(item);

            var result = await _service.GetBidById(bidId);

            Assert.That(result.Amount, Is.EqualTo(bid.Amount));
        }


    }
}
