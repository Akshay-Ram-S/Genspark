using Microsoft.AspNetCore.Mvc;
using UserApiApp.Models;
using UserApiApp.Repository;

namespace UserApiApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepo;

    public UserController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] User user)
    {
        var created = await _userRepo.AddUserAsync(user);
        return CreatedAtAction(nameof(AddUser), new { id = created.Id }, created);
    }
}
