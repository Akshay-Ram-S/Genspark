using System.Security.Claims;
using AuctionAPI.Interfaces;
using AuctionAPI.Models.DTOs;
using AuctionAPI.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class ItemsController : Controller
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ItemsController> _logger;
        private readonly IFunctionalities _functionalities;

        public ItemsController(ILogger<ItemsController> logger,
                               IItemService itemService,
                               IFunctionalities functionalities)
        {
            _itemService = itemService;
            _logger = logger;
            _functionalities = functionalities;
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateItem([FromForm] ItemCreateDto dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";

            _logger.LogInformation("User {User} initiated item creation", userEmail);

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (dto.EndDate < today)
            {
                _logger.LogWarning("Item creation failed by user {User}: EndDate {EndDate} < Today {Today}", userEmail, dto.EndDate, today);
                return BadRequest(ApiResponseMapper.BadRequest<string>("EndDate must not be before StartDate."));
            }

            try
            {
                var item = await _itemService.CreateItemAsync(dto);
                _logger.LogInformation("Item {ItemId} created successfully by user {User}", item.ItemID, userEmail);
                return CreatedAtAction(nameof(GetItem), new { id = item.ItemID },
                    ApiResponseMapper.Created(item, "Item created successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating item by user {User}", userEmail);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetItem(Guid id)
        {
            _logger.LogInformation("Request to fetch item {ItemId}", id);

            try
            {
                var result = await _itemService.GetItemById(id);
                if (result == null)
                {
                    _logger.LogWarning("Item {ItemId} not found", id);
                    return NotFound(ApiResponseMapper.NotFound<string>($"No item found with id: {id}"));
                }

                _logger.LogInformation("Item {ItemId} fetched successfully", id);
                return Ok(ApiResponseMapper.Success(result, "Item fetched successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching item {ItemId}", id);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllItems([FromQuery] ItemFilter? filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {

            try
            {
                page = Math.Max(1, page);
                pageSize = Math.Min(pageSize, 100);

                var result = await _itemService.GetItems(filter, page, pageSize);
                if (result == null)
                {
                    _logger.LogWarning("No items found in database");
                    return NotFound(ApiResponseMapper.NotFound<string>("No items found in the database."));
                }
                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No items found for the given filter");
                    return NotFound(ApiResponseMapper.NotFound<string>("No items found for the given filter."));
                }
                return Ok(new
                {
                    success = true,
                    message = "Items fetched successfully.",
                    data = result.Data,
                    pagination = result.Pagination,
                    errors = (object?)null
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching all items");
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [HttpGet("all-bids/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllBids(Guid id)
        {
            _logger.LogInformation("Fetching all bids for item {ItemId}", id);

            try
            {
                var result = await _functionalities.AllBids(id);
                if (result == null || !result.Any())
                {
                    _logger.LogWarning("No bids found for item {ItemId}", id);
                    return NotFound(ApiResponseMapper.NotFound<string>($"No bids found for the item with id: {id}"));
                }

                _logger.LogInformation("{BidCount} bids fetched for item {ItemId}", result.Count(), id);
                return Ok(ApiResponseMapper.Success(result, "Bids fetched successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching bids for item {ItemId}", id);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateItemById(Guid id, [FromForm] ItemUpdateDto item)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Updating item {ItemId}", id);
            try
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
                if (item.EndDate < today)
                {
                    _logger.LogWarning("Item creation failed by user {User}: EndDate {EndDate} <= Today {Today}", userEmail, item.EndDate, today);
                    return BadRequest(ApiResponseMapper.BadRequest<string>("EndDate must not be before StartDate."));
                }

                var result = await _itemService.UpdateItem(id, item, userEmail);
                if (result == null)
                    return StatusCode(403, ApiResponseMapper.Forbidden<string>("User not authorized to update this item."));
                return Ok(ApiResponseMapper.Success(result, "Item updated successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating item {ItemId} by user {User}", id, userEmail);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteItemById(Guid id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            try
            {
                var result = await _itemService.DeleteItem(id, userEmail);
                if (result == null)
                {
                    _logger.LogWarning("User {User} is not authorized to cancel item {ItemId}", userEmail, id);
                    return StatusCode(403, ApiResponseMapper.Forbidden<string>("User not authorized to cancel this item."));
                }

                _logger.LogInformation("Item {ItemId} cancelled successfully by user {User}", id, userEmail);
                return Ok(ApiResponseMapper.Success(result, "Item deleted successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error cancelling item {ItemId} by user {User}", id, userEmail);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }
    }
}
