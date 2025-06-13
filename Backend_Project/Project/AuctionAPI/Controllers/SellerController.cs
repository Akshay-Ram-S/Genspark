using AuctionAPI.Models.DTOs;
using AuctionAPI.Models;
using Microsoft.AspNetCore.Mvc;
using AuctionAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using AuctionAPI.Mappers;
using AuctionAPI.Misc;

namespace AuctionAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class SellerController : ControllerBase
    {
        private readonly IUserService<Seller> _sellerService;
        private readonly IUserService<Bidder> _bidderService;
        private readonly ILogger<SellerController> _logger;
        private readonly IValidation _validation;
        private readonly IFunctionalities _functionalities;

        public SellerController(IUserService<Seller> sellerService,
                                IUserService<Bidder> bidderService,
                                ILogger<SellerController> logger,
                                IValidation validation,
                                IFunctionalities functionalities)
        {
            _sellerService = sellerService;
            _bidderService = bidderService;
            _logger = logger;
            _validation = validation;
            _functionalities = functionalities;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSellers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Fetching all sellers - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            try
            {
                var users = await _sellerService.GetAllUsers(page, pageSize);
                _logger.LogInformation("Fetched {SellerCount} sellers", users.Count());
                return Ok(ApiResponseMapper.Success<object>(users, "Sellers fetched successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching sellers");
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSellerById(Guid id)
        {
            _logger.LogInformation("Fetching seller by ID: {SellerId}", id);

            try
            {
                var user = await _sellerService.GetUser(id);
                if (user == null)
                {
                    _logger.LogWarning("Seller with ID {SellerId} not found", id);
                    return NotFound(ApiResponseMapper.NotFound<string>($"Seller with id {id} not found"));
                }

                _logger.LogInformation("Seller {SellerId} fetched successfully", id);
                return Ok(ApiResponseMapper.Success(user, "Seller fetched successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching seller by ID: {SellerId}", id);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [HttpGet("Items/{id}")]
        public async Task<IActionResult> ItemsBySeller(Guid id)
        {
            try
            {
                var items = await _functionalities.ItemsBySeller(id);
                _logger.LogInformation($"Items posted by seller with id: {id} is fetched");
                return Ok(ApiResponseMapper.Success(items, $"Items by seller with id: {id} fetched successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while fetching seller by ID: {id}");
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }


    
    }
}
