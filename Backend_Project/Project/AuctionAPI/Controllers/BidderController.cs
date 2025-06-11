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

        [HttpPost]
        public async Task<IActionResult> CreateBidder(AddUserDto user)
        {
            _logger.LogInformation($"Attempting to register bidder with email: {user.Email}");

            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    _logger.LogWarning($"Authenticated user tried to register a bidder with email: {user.Email}");
                    return StatusCode(403, ApiResponseMapper.Forbidden<string>("You are already logged in."));
                }

                if (await _validation.EmailExists(user.Email))
                {
                    _logger.LogWarning($"Bidder registration failed: Email already exists - {user.Email}");
                    return Conflict(ApiResponseMapper.Conflict<string>("Email already exists."));
                }
                
                if (!_validation.ValidName(user.Name))
                {
                    _logger.LogWarning($"Bidder registration failed: Email already exists - {user.Name}");
                }

                if (!await _validation.ValidAadhar(user.Aadhar))
                {
                    _logger.LogWarning($"Bidder registration failed: Invalid Aadhar number - {user.Aadhar}");
                    return BadRequest(ApiResponseMapper.BadRequest<string>("Invalid Aadhar number."));
                }
                
                if (!await _validation.ValidPAN(user.PAN.ToUpper()))
                {
                    _logger.LogWarning($"Bidder registration failed: Invalid PAN number - {user.PAN}");
                    return BadRequest(ApiResponseMapper.BadRequest<string>("Invalid PAN number."));
                }

                var result = await _bidderService.AddUser(user);
                _logger.LogInformation("Bidder created successfully with ID: {BidderId}", result.BidderId);
                return Ok(ApiResponseMapper.Success(result, "Bidder created successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while creating bidder with email: {Email}", user.Email);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [HttpGet]
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

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBidder(Guid id, UpdateUserDto user)
        {
            _logger.LogInformation("Updating bidder with ID: {BidderId}", id);

            try
            {
                var updated = await _bidderService.UpdateUser(id, user);
                _logger.LogInformation($"Bidder {id} updated successfully");
                return Ok(ApiResponseMapper.Success(updated, "Bidder updated successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while updating bidder with ID: {id}");
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [HttpDelete("{id}")]
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
