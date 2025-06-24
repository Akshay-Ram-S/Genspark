using AuctionAPI.Contexts;
using AuctionAPI.Interfaces;
using AuctionAPI.Mappers;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionAPI.Misc
{
    public class Functionalities : IFunctionalities
    {
        private readonly AuctionContext _auctionContext;
        private readonly IValidation _validation;
        private readonly IEncryptionService _encryptionService;
        private readonly UserMapper _mapper;
        private readonly IRepository<string, User> _userRepository;

        public Functionalities(AuctionContext auctionContext,
                               IEncryptionService encryptionService,
                               IValidation validationService,
                               IRepository<string, User> userRepository)
        {
            _validation = validationService;
            _encryptionService = encryptionService;
            _mapper = new UserMapper();
            _auctionContext = auctionContext;
            _userRepository = userRepository;
        }

        public async Task<User> RegisterUser(AddUserDto user)
        {

            var _user = _mapper.MapUser(user);

            var encryptedPassword = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = user.Password
            });
            var encryptedPAN = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = user.PAN
            });
            var encryptedAadhar = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = user.Aadhar
            });
            _user.Password = encryptedPassword.EncryptedData;
            _user.PAN = encryptedPAN.EncryptedData;
            _user.Aadhar = encryptedAadhar.EncryptedData;
            return _user;
        }


        public async virtual Task<IEnumerable<ItemAllBids>> AllBids(Guid id)
        {
            var result = await _auctionContext.GetBidsByItem(id);
            if (result == null)
                throw new Exception("No bids for the given item");
            return result;
        }

        public async Task<Item> GetItemWithBids(Guid id)
        {
            return await _auctionContext.Items.Where(i => i.IsDeleted == false)
                                                .Include(i => i.Bids)
                                                .FirstOrDefaultAsync(i => i.Id == id)
                                                ?? throw new Exception("Item not found");
        }

        public async Task<IEnumerable<ItemResponse>> ItemsBySeller(Guid sellerId)
        {
            var seller = await _auctionContext.Sellers
                .Include(s => s.User)
                .Include(s => s.Items!.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.ItemDetails!)
                        .ThenInclude(d => d.Bidder)
                .Where(s => s.SellerId == sellerId && s.User!.Status == "Active")
                .FirstOrDefaultAsync();

            if (seller == null || seller.Items == null || !seller.Items.Any())
            {
                throw new Exception("No items posted by the seller");
            }

            var itemSummaries = seller.Items.Select(item => new ItemResponse
            {
                ItemID = item.Id,
                Title = item.Title,
                Description = item.ItemDetails?.Description ?? "",
                Status = item.Status ?? "",
                Category = item.Category ?? "",
                StartingPrice = item.ItemDetails?.StartingPrice ?? 0,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                CurrentBid = item.ItemDetails?.CurrentBid,
                CurrentBidderName = item.ItemDetails?.Bidder?.User?.Name ?? "No bids yet"
            });

            return itemSummaries;
        }


        public async Task<IEnumerable<BidsByBidderDto>> BidsByBidder(Guid bidderId)
        {
            var bids = await _auctionContext.Bids
                            .Where(b => b.BidderId == bidderId && !b.IsDeleted && b.Bidder.User.Status == "Active")
                            .Include(b => b.Bidder)
                                .ThenInclude(bidder => bidder.User)
                            .Include(b => b.Item)
                            .ToListAsync();

            if (!bids.Any())
                throw new Exception("No bids made by the bidder");

            var itemIds = bids.Select(b => b.ItemId).Distinct().ToList();

            var itemMap = await _auctionContext.Items
                                .Where(i => itemIds.Contains(i.Id))      
                                .ToDictionaryAsync(i => i.Id, i => i.Title);


            var bidSummaries = bids.Select(bid => new BidsByBidderDto
            {
                BidderId = bid.BidderId,
                Name = bid.Bidder.User.Name,
                ItemId = bid.ItemId,
                Title = itemMap.ContainsKey(bid.ItemId) ? itemMap[bid.ItemId] : "Unknown",
                Amount = bid.Amount,
                Timestamp = bid.Timestamp
            });

            return bidSummaries;
        }

        public async Task<User> GetUserDetails(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            }

            var user = await _userRepository.Get(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return user;
        }
        
    }
}