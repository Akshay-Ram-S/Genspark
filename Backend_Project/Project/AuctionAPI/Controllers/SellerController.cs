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

        [HttpPost]
        public async Task<IActionResult> CreateSeller(AddUserDto user)
        {
            _logger.LogInformation($"Received request to create seller with email: {user.Email}");

            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    _logger.LogWarning($"Authenticated user tried to register a seller with email: {user.Email}");
                    return StatusCode(403, ApiResponseMapper.Forbidden<string>("You are already logged in."));
                }

                if (await _validation.EmailExists(user.Email))
                {
                    return Conflict(ApiResponseMapper.Conflict<string>("Email already exists."));
                }

                if (!_validation.ValidName(user.Name))
                {
                    return BadRequest(ApiResponseMapper.BadRequest<string>("Name must be more than 2 characters long."));
                }
                
                if (!await _validation.ValidAadhar(user.Aadhar))
                {
                    return BadRequest(ApiResponseMapper.BadRequest<string>("Invalid Aadhar number."));
                }

                if (!await _validation.ValidPAN(user.PAN.ToUpper()))
                {
                    return BadRequest(ApiResponseMapper.BadRequest<string>("Invalid PAN number."));
                }

                var result = await _sellerService.AddUser(user);
                _logger.LogInformation("Seller created successfully with ID: {SellerId}", result.SellerId);
                return Ok(ApiResponseMapper.Success(result, "Seller created successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while creating seller with email: {Email}", user.Email);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [HttpGet]
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

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateSeller(Guid id, UpdateUserDto user)
        {
            _logger.LogInformation("Updating seller with ID: {SellerId}", id);

            try
            {
                var updUser = await _sellerService.UpdateUser(id, user);
                _logger.LogInformation("Seller {SellerId} updated successfully", id);
                return Ok(ApiResponseMapper.Success(updUser, "Seller updated successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating seller {SellerId}", id);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeller(Guid id)
        {
            _logger.LogInformation("Deleting seller with ID: {SellerId}", id);

            try
            {
                var delSeller = await _sellerService.DeleteUser(id);
                _logger.LogInformation("Seller {SellerId} deleted successfully", id);
                return Ok(ApiResponseMapper.Success<object>(delSeller, "Seller deleted successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting seller {SellerId}", id);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }
    }
}
