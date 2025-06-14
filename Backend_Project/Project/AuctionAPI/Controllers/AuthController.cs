using AuctionAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using AuctionAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AuctionAPI.Mappers;

namespace AuctionAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthController> _logger;
        private readonly IFunctionalities _functionalities;

        public AuthController(IAuthenticationService authenticationService,
                              ILogger<AuthController> logger,
                              IFunctionalities functionalities)
        {
            _authenticationService = authenticationService;
            _logger = logger;
            _functionalities = functionalities;
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserLogin(UserLoginRequest loginRequest)
        {
            try
            {
                var result = await _authenticationService.Login(loginRequest);
                return Ok(ApiResponseMapper.Success(result, "Login successful"));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Login failed for user: {Email}", loginRequest.Email);
                return Unauthorized(ApiResponseMapper.Unauthorized<string>("Invalid email or password"));
            }
        }

        [Authorize]
        [HttpPost("Refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh([FromBody] TokenRefreshRequest request)
        {
            try
            {
                var newJwtToken = await _authenticationService.RefreshTokenAsync(request.RefreshToken);
                return Ok(ApiResponseMapper.Success(new { Token = newJwtToken }, "Token refreshed successfully."));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponseMapper.Unauthorized<string>("Invalid refresh token."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token.");
                return StatusCode(500, ApiResponseMapper.InternalError<string>(ex));
            }
        }

        [Authorize]
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserLogout([FromBody] TokenRefreshRequest token)
        {
            if (token == null || string.IsNullOrWhiteSpace(token.RefreshToken))
            {
                return BadRequest(ApiResponseMapper.Fail<string>("Invalid refresh token."));
            }

            try
            {
                await _authenticationService.Logout(token);
                return Ok(ApiResponseMapper.Success<string>(null, "Logged out successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed.");
                return StatusCode(500, ApiResponseMapper.InternalError<string>(ex));
            }
        }

        [Authorize]
        [HttpGet("Me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MyProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var result = await _functionalities.GetUserDetails(email);

                return Ok(ApiResponseMapper.Success(result, "User profile fetched."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to retrieve user profile.");
                return Unauthorized(ApiResponseMapper.Unauthorized<string>("Unable to access profile."));
            }
        }
    }
}
