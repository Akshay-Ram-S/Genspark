using AuctionAPI.Models.DTOs;
using AuctionAPI.Models;
using Microsoft.AspNetCore.Mvc;
using AuctionAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using AuctionAPI.Mappers;
using AuctionAPI.Misc;
using System.Security.Claims;


namespace AuctionAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService<Seller> _sellerService;
        private readonly IUserService<Bidder> _bidderService;
        private readonly ICommonUserService _userService;
        private readonly ILogger<SellerController> _logger;
        private readonly IValidation _validation;
        private readonly IFunctionalities _functionalities;

        public UserController(IUserService<Seller> sellerService,
                                IUserService<Bidder> bidderService,
                                ICommonUserService userService,
                                ILogger<SellerController> logger,
                                IValidation validation,
                                IFunctionalities functionalities)
        {
            _sellerService = sellerService;
            _bidderService = bidderService;
            _userService = userService;
            _logger = logger;
            _validation = validation;
            _functionalities = functionalities;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser(AddUserDto user)
        {
            _logger.LogInformation($"Received request to create seller with email: {user.Email}");

            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    _logger.LogWarning($"Authenticated user tried to register a seller with email: {user.Email}");
                    return StatusCode(403, ApiResponseMapper.Forbidden<string>("You are already logged in."));
                }

                if (!_validation.IsValidEmail(user.Email))
                {
                    _logger.LogWarning($"Bidder registration failed: Invalid email format - {user.Email}");
                    return BadRequest(ApiResponseMapper.BadRequest<string>("Invalid email format."));
                }

                if (!_validation.ValidName(user.Name))
                {
                    _logger.LogWarning($"Bidder registration failed: Name must be more than 2 characters long - {user.Name}");
                    return BadRequest(ApiResponseMapper.BadRequest<string>("Name must be more than 2 characters long."));
                }

                if (!await _validation.ValidAadharAndPAN(user.Aadhar, user.PAN.ToUpper()))
                {
                    _logger.LogWarning($"Bidder registration failed: Invalid Aadhar or PAN - Aadhar: {user.Aadhar}, PAN: {user.PAN}");
                    return BadRequest(ApiResponseMapper.BadRequest<string>("Invalid Aadhar or PAN number."));
                }

                if (await _validation.EmailExists(user.Email))
                {
                    _logger.LogWarning($"Attempt to register with existing email: {user.Email}");
                    return Conflict(ApiResponseMapper.Conflict<string>("Email already exists."));
                }
                if (await _validation.PhoneExists(user.Phone))
                {
                    _logger.LogWarning($"Attempt to register with existing phone: {user.Phone}");
                    return Conflict(ApiResponseMapper.Conflict<string>("Phone number already exists."));
                }
                
                if (await _validation.AadharExists(user.Aadhar))
                {
                    return Conflict(ApiResponseMapper.Conflict<string>("Aadhar already exists"));
                }
                if (await _validation.PanExists(user.PAN))
                {
                    return Conflict(ApiResponseMapper.Conflict<string>("PAN already exists"));
                }
                

                if (user.Role.ToLower() == "seller")
                {
                    var result = await _sellerService.AddUser(user);
                    _logger.LogInformation("Seller created successfully with ID: {SellerId}", result.SellerId);
                    return CreatedAtAction("GetSellerById", "Seller", new { id = result.SellerId },
                            ApiResponseMapper.Created(result, "Seller created successfully."));
                }
                else if (user.Role.ToLower() == "bidder")
                {
                    var result = await _bidderService.AddUser(user);
                    _logger.LogInformation("Bidder created successfully with ID: {BidderId}", result.BidderId);
                    return CreatedAtAction("GetBidderById", "Bidder", new { id = result.BidderId },
                            ApiResponseMapper.Created(result, "Bidder created successfully."));
                }
                // else if (user.Role.ToLower() == "admin")
                // {
                //     var result = await _userService.CreateAdmin(user);
                //     _logger.LogWarning($"Invalid role specified: {user.Role}");
                //     return Ok(ApiResponseMapper.Success(result, $"Admin created"));
                // }
                
                return Ok(ApiResponseMapper.Success("Done", $"Admin created"));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while creating user with email: {Email}", user.Email);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [Authorize]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto user)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            try
            {
                if (email == null)
                {
                    return Unauthorized(ApiResponseMapper.Unauthorized<string>());
                }

                var updatedUser = await _userService.UpdateUser(email, user);

                if (updatedUser == null)
                {
                    _logger.LogWarning($"Update failed. No user found with ID: {email}");
                    return NotFound(ApiResponseMapper.NotFound<string>($"{email} not found"));
                }

                _logger.LogInformation($"User with email: {email} updated successfully");
                return Ok(ApiResponseMapper.Success(updatedUser, $"{email} updated successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while updating User with Email: {email}");
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [Authorize]
        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser()
        {

            try
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (email == null)
                {
                    return Unauthorized(ApiResponseMapper.Unauthorized<string>());
                }

                var deletedUser = await _userService.DeleteUser(email);

                if (deletedUser == null)
                {
                    _logger.LogWarning("Delete failed");
                    return NotFound(ApiResponseMapper.NotFound<string>("User not found."));
                }

                _logger.LogInformation($"User with email: {email} deleted successfully");
                return Ok(ApiResponseMapper.Success(deletedUser, $"User deleted successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting User");
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("change-state")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeUserStatus([FromBody]UserStatusUpdate statusDto)
        {

            try
            {
                if (statusDto.Email == null)
                {
                    return Unauthorized(ApiResponseMapper.Unauthorized<string>());
                }

                var deletedUser = await _userService.ChangeUserState(statusDto);

                if (deletedUser == null)
                {
                    _logger.LogWarning("Delete failed");
                    return NotFound(ApiResponseMapper.NotFound<string>("User not found."));
                }

                _logger.LogInformation($"User with email: {statusDto.Email} {statusDto.Status} successfully");
                return Ok(ApiResponseMapper.Success(deletedUser, $"User status changed successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting User");
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }    


    }
}
