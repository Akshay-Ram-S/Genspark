using AuctionAPI.Models.DTOs;
using AuctionAPI.Models;
using Microsoft.AspNetCore.Mvc;
using AuctionAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using AuctionAPI.Mappers;

namespace AuctionAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class BidderController : ControllerBase
    {
        private readonly IUserService<Bidder> _bidderService;
        private readonly ILogger<BidderController> _logger;
        private readonly IValidation _validation;
        private readonly IFunctionalities _functionalities;

        public BidderController(IUserService<Bidder> bidderService,
                                ILogger<BidderController> logger,
                                IValidation validation,
                                IFunctionalities functionalities)
        {
            _bidderService = bidderService;
            _logger = logger;
            _validation = validation;
            _functionalities = functionalities;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBidders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Fetching bidders - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            try
            {
                var users = await _bidderService.GetAllUsers(page, pageSize);
                _logger.LogInformation("Fetched {Count} bidders", users.Count());
                return Ok(ApiResponseMapper.Success(users, "Fetched all bidders."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching bidders");
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBidderById(Guid id)
        {
            _logger.LogInformation("Fetching bidder with ID: {BidderId}", id);

            try
            {
                var bidder = await _bidderService.GetUser(id);
                if (bidder == null)
                {
                    _logger.LogWarning("Bidder not found with ID: {BidderId}", id);
                    return NotFound(ApiResponseMapper.NotFound<string>("Bidder not found."));
                }

                _logger.LogInformation("Bidder {BidderId} fetched successfully", id);
                return Ok(ApiResponseMapper.Success(bidder, "Fetched bidder."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching bidder with ID: {BidderId}", id);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [HttpGet("Bids/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BidsByBidder(Guid id)
        {
            try
            {
                var items = await _functionalities.BidsByBidder(id);
                _logger.LogInformation($"Bids posted by bidder with id: {id} is fetched");
                return Ok(ApiResponseMapper.Success(items, $"Bids posted by bidder with id: {id} fetched successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while fetching bids by Bidder ID: {id}");
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }


        [Authorize(Roles = "Bidder")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBidder(Guid id)
        {
            _logger.LogInformation("Deleting bidder with ID: {BidderId}", id);

            try
            {
                var deleted = await _bidderService.DeleteUser(id);
                if (deleted == null)
                {
                    _logger.LogWarning("Delete failed: Bidder not found with ID: {BidderId}", id);
                    return NotFound(ApiResponseMapper.NotFound<string>("Bidder not found."));
                }

                _logger.LogInformation("Bidder {BidderId} deleted successfully", id);
                return Ok(ApiResponseMapper.Success(deleted, "Bidder deleted successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting bidder with ID: {BidderId}", id);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }
    }
}
