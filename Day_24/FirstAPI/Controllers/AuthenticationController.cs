
using FirstAPI.Interfaces;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using FirstAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using FirstAPI.Misc;
using FirstAPI.Repositories;


namespace FirstAPI.Controllers
{


    [ApiController]
    [Route("/api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthenticationService authenticationService, ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        /*
        [HttpPost]
        public async Task<ActionResult<UserLoginResponse>> UserLogin(UserLoginRequest loginRequest)
        {
            
            try
            {
                var result = await _authenticationService.Login(loginRequest);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Unauthorized(e.Message);
            }
            
            //return await _authenticationService.Login(loginRequest);

        }
        */

        [HttpPost]
        public async Task<ActionResult<UserLoginResponse>> UserLogin(string token)
        {
            var result = await _authenticationService.LoginWithGoogle(token);
            return Ok(result);
        }

        // [HttpGet]
        // public IActionResult Login()
        // {
        //     var redirectUrl = Url.Action("response", "Authentication");
        //     var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        //     properties.Items["prompt"] = "select_account";

        //     return Challenge(properties, "Google");

        // }

    }

}