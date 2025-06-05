
using Microsoft.AspNetCore.Mvc;
using DocSharingAPI.Interfaces;
using DocSharingAPI.Models;
using DocSharingAPI.Models.DTOs;

namespace DocSharingAPI.Controllers
{


    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] User user)
        {
            try
            {
                var newUser = await _userService.AddUser(user);
                if (newUser != null)
                    return Created("", newUser);
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}