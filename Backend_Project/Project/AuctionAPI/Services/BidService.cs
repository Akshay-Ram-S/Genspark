
using System.Collections.ObjectModel;
using AuctionAPI.Hubs;
using AuctionAPI.Interfaces;
using AuctionAPI.Mappers;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using Microsoft.AspNetCore.SignalR;
#pragma warning disable CS8603 // Possible null reference return.
namespace AuctionAPI.Services
{
    public class BidService : IBidService
    {
        private readonly BidMapper _mapper;
        private readonly BidResponseMapper _mapperResponse;
        private readonly IRepository<Guid, Item> _itemRepository;
        private readonly IRepository<Guid, ItemDetails> _itemDetailsRepository;
        private readonly IRepository<Guid, Bidder> _bidderRepository;
        private readonly IRepository<Guid, Bid> _bidRepository;
        private readonly IHubContext<AuctionHub> _hubContext;

        public BidService(IRepository<Guid, Bid> bidRepository,
                            IRepository<Guid, Bidder> bidderRepository,
                            IRepository<Guid, Item> itemRepository,
                            IRepository<Guid, ItemDetails> itemDetailsRepository,
                            IHubContext<AuctionHub> hubContext)
        {
            _mapper = new BidMapper();
            _mapperResponse = new BidResponseMapper();
            _itemRepository = itemRepository;
            _itemDetailsRepository = itemDetailsRepository;
            _bidderRepository = bidderRepository;
            _bidRepository = bidRepository;
            _hubContext = hubContext;
        }

        public async Task<BidResponse> PlaceBid(BidCreateDTO bidDto)
        {
            try
            {
                if (bidDto == null)
                    throw new ArgumentNullException(nameof(bidDto), "Bid data cannot be null.");
                if (bidDto.ItemId == Guid.Empty)
                    throw new ArgumentException("ItemId must be a valid GUID.", nameof(bidDto.ItemId));
                if (bidDto.BidderId == Guid.Empty)
                    throw new ArgumentException("BidderId must be a valid GUID.", nameof(bidDto.BidderId));

                var item = await _itemRepository.Get(bidDto.ItemId);
                if (item == null)
                    throw new Exception("Item not found");

                var bidder = await _bidderRepository.Get(bidDto.BidderId);
                if (bidder == null)
                    throw new Exception("Bidder not found");

                var itemDetails = await _itemDetailsRepository.Get(bidDto.ItemId);
                if (itemDetails == null)
                    throw new Exception("ItemDetails not found for this item");

                await IsBidAmountHigh(bidDto, item, itemDetails);

                item.Bids ??= new Collection<Bid>();
                bidder.Bids ??= new List<Bid>();

                var bid = _mapper.MapBid(bidDto);
                if (bid == null)
                    throw new Exception("Failed to map BidCreateDTO to Bid entity.");

                bid.Item = item;
                bid.Bidder = bidder;

                item.Bids.Add(bid);
                bidder.Bids.Add(bid);

                itemDetails.CurrentBid = bid.Amount;
                itemDetails.CurrentBidderID = bidder.BidderId;

                await _bidRepository.Add(bid);
                await _itemRepository.Update(item.Id, item);
                await _itemDetailsRepository.Update(itemDetails.ItemId, itemDetails);

                if (bidder.User == null)
                    throw new Exception("Bidder's user data not found.");
                if (_hubContext == null)
                    throw new InvalidOperationException("SignalR hub context is not initialized.");
                await _hubContext.Clients.All.SendAsync("ReceiveBid", new
                {
                    ItemId = item.Id,
                    Title = item.Title,
                    Amount = bid.Amount,
                    Bidder = bidder.User.Name,
                    Time = DateTime.UtcNow
                });

                var bidResponse = _mapperResponse.MapBidResponse(bid, bidder, item);
                return bidResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message); 
            }
        }

        private async Task IsBidAmountHigh(BidCreateDTO bidDto, Item item, ItemDetails itemDetails)
        {
            if (bidDto.Amount < itemDetails.StartingPrice)
            {
                throw new Exception($"Bid value must be greater than start value: {itemDetails.StartingPrice}");
            }
            var highestAmount = await HighestBidAmount(item.Id);

            if (highestAmount.HasValue && bidDto.Amount <= highestAmount)
            {
                throw new Exception($"Bid must be greater than the current highest bid {highestAmount}");
            }
        }

        private async Task<decimal?> HighestBidAmount(Guid itemId)
        {
            var allBids = await _bidRepository.GetAll();
            var highestBid = allBids
                .Where(b => b.ItemId == itemId && !b.IsDeleted)
                .OrderByDescending(b => b.Amount)
                .FirstOrDefault();
            return highestBid?.Amount;
        }

        public async Task<BidResponse> GetBidById(Guid id)
        {
            try
            {
                var bid = await _bidRepository.Get(id);
                var bidder = await _bidderRepository.Get(bid.BidderId);
                var item = await _itemRepository.Get(bid.ItemId);

                if (bid == null)
                {
                    throw new Exception("No such item found");
                }
                var bidResponse = _mapperResponse.MapBidResponse(bid, bidder, item);

                return bidResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<BidResponse?> CancelBid(Guid bidId, string bidderEmail)
        {
            try
            {
                var bid = await _bidRepository.Get(bidId);
                var bidder = await _bidderRepository.Get(bid.BidderId);
                var item = await _itemRepository.Get(bid.ItemId);
                if (bid == null || bid.Bidder == null || bidder.User == null || bidder.User.Email != bidderEmail)
                {
                    return null;
                }
                var highestBid = await HighestBidAmount(item.Id);
                
                if (bid.Amount != highestBid)
                {
                    throw new Exception("Your bid is not the highest bid");
                }

                bid.IsDeleted = true;
                bid = await _bidRepository.Update(bidId, bid);
                var bidResponse = _mapperResponse.MapBidResponse(bid, bidder, item);
                return bidResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }


    }
}
#pragma warning restore CS8603 // Possible null reference return.