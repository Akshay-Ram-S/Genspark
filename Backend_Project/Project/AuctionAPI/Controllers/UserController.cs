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
        private readonly ILogger<SellerController> _logger;
        private readonly IValidation _validation;
        private readonly IFunctionalities _functionalities;

        public UserController(IUserService<Seller> sellerService,
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser(AddUserDto user, string role)
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

                if (role == "Seller")
                {
                    var result = await _sellerService.AddUser(user);
                    _logger.LogInformation("Seller created successfully with ID: {SellerId}", result.SellerId);
                    return CreatedAtAction("GetSellerById", "Seller", new { id = result.SellerId },
                            ApiResponseMapper.Created(result, "Seller created successfully."));
                }
                else if (role == "Bidder")
                {
                    var result = await _bidderService.AddUser(user);
                    _logger.LogInformation("Bidder created successfully with ID: {BidderId}", result.BidderId);
                    return CreatedAtAction("GetBidderById", "Bidder", new { id = result.BidderId },
                            ApiResponseMapper.Created(result, "Bidder created successfully."));
                }
                else
                {
                    _logger.LogWarning("Invalid role specified: {Role}", role);
                    return BadRequest(ApiResponseMapper.BadRequest<string>("Invalid role specified."));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while creating user with email: {Email}", user.Email);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserByRole(Guid id, UpdateUserDto user)
        {
            var jwtEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            try
            {
                if (jwtEmail == null)
                {
                    return Unauthorized(ApiResponseMapper.Unauthorized<string>());
                }

                object? existingUser = role switch
                {
                    "Seller" => await _sellerService.GetUser(id),
                    "Bidder" => await _bidderService.GetUser(id),
                    _ => null
                };

                if (existingUser is null)
                    return NotFound(ApiResponseMapper.NotFound<string>($"{role} not found."));

                var emailFromDb = role switch
                {
                    "Seller" => ((Seller)existingUser).User.Email,
                    "Bidder" => ((Bidder)existingUser).User.Email,
                    _ => null
                };

                if (!string.Equals(jwtEmail, emailFromDb, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }
                object? updatedUser = role switch
                {
                    "Seller" => await _sellerService.UpdateUser(id, user),
                    "Bidder" => await _bidderService.UpdateUser(id, user),
                    _ => null
                };

                if (updatedUser == null)
                {
                    _logger.LogWarning("{Role} update failed. No user found with ID: {UserId}", role, id);
                    return NotFound(ApiResponseMapper.NotFound<string>($"{role} with ID {id} not found"));
                }

                _logger.LogInformation("{Role} {UserId} updated successfully", role, id);
                return Ok(ApiResponseMapper.Success(updatedUser, $"{role} updated successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating {Role} with ID: {UserId}", role, id);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserByRole(Guid id)
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            _logger.LogInformation("Deleting {Role} with ID: {UserId}", role, id);
            try
            {
                var jwtEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (jwtEmail == null)
                {
                    return Unauthorized(ApiResponseMapper.Unauthorized<string>());
                }

                object? existingUser = role switch
                {
                    "Seller" => await _sellerService.GetUser(id),
                    "Bidder" => await _bidderService.GetUser(id),
                    _ => null
                };

                if (existingUser is null)
                    return NotFound(ApiResponseMapper.NotFound<string>($"{role} not found."));

                var emailFromDb = role switch
                {
                    "Seller" => ((Seller)existingUser).User.Email,
                    "Bidder" => ((Bidder)existingUser).User.Email,
                    _ => null
                };

                if (!string.Equals(jwtEmail, emailFromDb, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                object? deletedUser = role switch
                {
                    "Seller" => await _sellerService.DeleteUser(id),
                    "Bidder" => await _bidderService.DeleteUser(id),
                    _ => null
                };

                if (deletedUser == null)
                {
                    _logger.LogWarning("Delete failed: {Role} not found with ID: {UserId}", role, id);
                    return NotFound(ApiResponseMapper.NotFound<string>($"{role} not found."));
                }

                _logger.LogInformation("{Role} {UserId} deleted successfully", role, id);
                return Ok(ApiResponseMapper.Success(deletedUser, $"{role} deleted successfully."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting {Role} with ID: {UserId}", role, id);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(e));
            }
        }



    }
}
