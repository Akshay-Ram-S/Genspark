using System.Text.RegularExpressions;
using AuctionAPI.Interfaces;
using AuctionAPI.Mappers;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using FuzzySharp;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.

namespace AuctionAPI.Services
{
    public class ItemService : IItemService
    {
        private readonly IRepository<Guid, Item> _itemRepository;
        private readonly IRepository<Guid, ItemDetails> _itemDetailsRepository;
        private readonly IRepository<Guid, Bidder> _bidderRepository;
        private readonly IRepository<Guid, Audit> _auditRepository;
        private readonly ItemMapper _mapper;
        private readonly ItemResponseMapper _mapperResponse;
        private readonly IRepository<Guid, Seller> _sellerRepository;

        public ItemService(IRepository<Guid, Item> itemRepository,
                            IRepository<Guid, Seller> sellerRepository,
                            IRepository<Guid, ItemDetails> itemDetailsRepository,
                            IRepository<Guid, Bidder> bidderRepository,
                            IRepository<Guid, Audit> auditRepository)
        {
            _mapper = new ItemMapper();
            _mapperResponse = new ItemResponseMapper();
            _sellerRepository = sellerRepository;
            _itemRepository = itemRepository;
            _itemDetailsRepository = itemDetailsRepository;
            _bidderRepository = bidderRepository;
            _auditRepository = auditRepository;
        }

        public async Task<ItemResponse> CreateItemAsync(ItemCreateDto dto)
        {
            try
            {
                var item = _mapper.MapItem(dto);

                var seller = await _sellerRepository.Get(dto.SellerID);
                if (seller == null)
                {
                    throw new Exception($"Seller with ID {dto.SellerID} not found");
                }

                item = await _itemRepository.Add(item);

                var itemDetails = new ItemDetails
                {
                    ItemId = item.Id,
                    StartingPrice = dto.StartingPrice,
                    Description = dto.Description,
                    CurrentBid = 0
                };

                if (dto.Image != null && dto.Image.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await dto.Image.CopyToAsync(ms);
                    itemDetails.ImageData = ms.ToArray();
                    itemDetails.ImageMimeType = dto.Image.ContentType;
                }

                await _itemDetailsRepository.Add(itemDetails);

                seller.Items ??= new List<Item>();
                seller.Items.Add(item);
                await _sellerRepository.Update(seller.SellerId, seller);
                await _auditRepository.Add(new Audit
                {
                    Action = "Create",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = seller.User.Email,
                    EntityId = item.Id,
                    EntityType = "Item"
                });

                var itemResponse = _mapperResponse.MapItemResponse(item, itemDetails, seller, null);
                return itemResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public async Task<ItemResponse?> GetItemById(Guid id)

        {
            try
            {
                var item = await _itemRepository.Get(id);
                if (item == null)
                {
                    throw new Exception("No such item found");
                }

                var itemDetail = await _itemDetailsRepository.Get(id);
                var seller = await _sellerRepository.Get(item.SellerID);

                Bidder? bidder = null;
                if (itemDetail.CurrentBidderID.HasValue)
                {
                    bidder = await _bidderRepository.Get(itemDetail.CurrentBidderID.Value);
                }

                var itemResponse = _mapperResponse.MapItemResponse(item, itemDetail, seller, bidder);
                return itemResponse;
            }
            catch (Exception e)
            {
                throw new Exception($"Error retrieving item: {e.Message}");
            }
        }


       public async Task<PagedResult<ItemResponse>> GetItems(ItemFilter filter, int page = 1, int pageSize = 10)
        {
            try
            {
                var allItems = await _itemRepository.GetAll();
                if (allItems == null || !allItems.Any())
                {
                    throw new Exception("No items in the database");
                }

                var filteredItems = allItems.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.Category))
                    filteredItems = filteredItems.Where(i => i.Category == filter.Category);

                if (filter.PriceLessThan.HasValue)
                    filteredItems = filteredItems.Where(i => i.ItemDetails.CurrentBid <= filter.PriceLessThan.Value);

                if (filter.EndDateBefore.HasValue)
                    filteredItems = filteredItems.Where(i => i.EndDate < filter.EndDateBefore.Value);

                var inMemoryItems = filteredItems.ToList();

                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    var searchTerm = filter.Search.Trim();
                    inMemoryItems = inMemoryItems
                        .Where(i => IsFuzzyMatch(searchTerm, i.Title))
                        .ToList();
                }

                var totalRecords = inMemoryItems.Count;
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                var pagedItems = inMemoryItems
                    .OrderBy(i => i.EndDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var itemResponses = new List<ItemResponse>();
                foreach (var item in pagedItems)
                {
                    var itemDetail = await _itemDetailsRepository.Get(item.Id);
                    var seller = await _sellerRepository.Get(item.SellerID);

                    Bidder? bidder = null;
                    if (itemDetail.CurrentBidderID.HasValue)
                    {
                        bidder = await _bidderRepository.Get(itemDetail.CurrentBidderID.Value);
                    }

                    var itemResponse = _mapperResponse.MapItemResponse(item, itemDetail, seller, bidder);
                    itemResponses.Add(itemResponse);
                }

                return new PagedResult<ItemResponse>
                {
                    Data = itemResponses,
                    Pagination = new PaginationMetadata
                    {
                        TotalRecords = totalRecords,
                        Page = page,
                        PageSize = pageSize,
                        TotalPages = totalPages
                    }
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Error retrieving items: {e.Message}");
            }
        }

        public static bool IsFuzzyMatch(string source, string target, int threshold = 70)
        {
            string Clean(string input) => Regex.Replace(input.Trim().ToLower(), @"\s+", " ");

            source = Clean(source);
            target = Clean(target);

            int ratio = Fuzz.Ratio(source, target);
            int tokenRatio = Fuzz.TokenSetRatio(source, target);
            int partialRatio = Fuzz.PartialRatio(source, target);

            double avgScore = (tokenRatio + partialRatio + ratio) / 3.0;

            return avgScore >= threshold;
        }


        public async Task<Item> DeleteItem(Guid id, string userEmail)
        {
            try
            {
                var item = await _itemRepository.Get(id);
                var seller = await _sellerRepository.Get(item.SellerID);
                if (item == null || item.Seller == null || seller.User == null || seller.User.Email != userEmail)
                {
                    return null;
                }
                item.IsDeleted = true;
                item = await _itemRepository.Update(id, item);

                await _auditRepository.Add(new Audit
                {
                    Action = "Delete",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = seller.User.Email,
                    EntityId = item.Id,
                    EntityType = "Item"
                });
                return item;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<ItemResponse> UpdateItem(Guid itemId, ItemUpdateDto dto, string userEmail)
        {
            try
            {
                var item = await _itemRepository.Get(itemId);
                if (item == null)
                {
                    throw new Exception($"Item with ID {itemId} not found");
                }

                var seller = await _sellerRepository.Get(item.SellerID);
                if (item.Seller == null || seller.User.Email != userEmail)
                {
                    return null;
                }

                var itemDetails = await _itemDetailsRepository.Get(itemId);
                if (itemDetails == null)
                {
                    throw new Exception($"ItemDetails for Item ID {itemId} not found");
                }

                item.Title = dto.Title;
                item.EndDate = dto.EndDate;
                item.Category = dto.Category;
                itemDetails.Description = dto.Description;

                if (dto.Image != null && dto.Image.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await dto.Image.CopyToAsync(ms);
                    itemDetails.ImageData = ms.ToArray();
                    itemDetails.ImageMimeType = dto.Image.ContentType;
                }

                await _itemRepository.Update(item.Id, item);
                await _itemDetailsRepository.Update(itemDetails.ItemId, itemDetails);

                var itemResponse = _mapperResponse.MapItemResponse(item, itemDetails, seller, null);
                return itemResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}

#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.