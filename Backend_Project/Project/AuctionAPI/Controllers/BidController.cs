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
    
    public class BidController : Controller
    {
        private readonly IBidService _bidService;
        private readonly ILogger<BidController> _logger;

        public BidController(ILogger<BidController> logger,
                             IBidService bidService)
        {
            _bidService = bidService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Bidder")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostBid(BidCreateDTO bidDto)
        {
            _logger.LogInformation("Attempting to place a bid on item {ItemId} by bidder {BidderId} for amount {Amount}",
                bidDto.ItemId, bidDto.BidderId, bidDto.Amount);

            try
            {
                var bid = await _bidService.PlaceBid(bidDto);

                _logger.LogInformation("Bid placed successfully with ID {BidId}", bid.Id);

                return CreatedAtAction(nameof(GetBid), new { id = bid.Id },
                    ApiResponseMapper.Success(bid, "Bid placed successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing bid on item {ItemId} by bidder {BidderId}",
                    bidDto.ItemId, bidDto.BidderId);

                return BadRequest(ApiResponseMapper.Fail<string>(ex.Message));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBid(Guid id)
        {
            _logger.LogInformation("Fetching bid with ID: {BidId}", id);

            try
            {
                var result = await _bidService.GetBidById(id);

                if (result == null)
                {
                    _logger.LogWarning("Bid not found with ID: {BidId}", id);
                    return NotFound(ApiResponseMapper.NotFound<string>($"No bid found with id: {id}"));
                }

                _logger.LogInformation("Bid with ID {BidId} fetched successfully", id);

                return Ok(ApiResponseMapper.Success(result, "Bid fetched successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching bid with ID: {BidId}", id);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Bidder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelBid(Guid id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Unauthorized cancel attempt for bid {BidId} - email claim missing", id);
                return Unauthorized(ApiResponseMapper.Fail<string>("Invalid token: email not found."));
            }

            try
            {
                var result = await _bidService.CancelBid(id, email);

                if (result == null)
                {
                    _logger.LogWarning("Unauthorized cancel attempt: User {Email} is not the owner of bid {BidId}", email, id);
                    return StatusCode(403, ApiResponseMapper.Forbidden<string>("User not authorized to cancel this bid."));
                }

                _logger.LogInformation("Bid {BidId} cancelled successfully by user {Email}", id, email);

                return Ok(ApiResponseMapper.Success(result, "Bid cancelled successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error cancelling bid {BidId} by user {Email}", id, email);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }
    }
}
