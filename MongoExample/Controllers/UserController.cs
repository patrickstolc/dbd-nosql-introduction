using Microsoft.AspNetCore.Mvc;
using MongoExample.Core.Services;

namespace MongoExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPut("{userId}/username")]
    public IActionResult UpdateUsername(string userId, [FromBody] UpdateUsernameRequest request)
    {
        try
        {
            _userService.UpdateUserName(userId, request.NewUsername);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

public class UpdateUsernameRequest
{
    public string NewUsername { get; set; }
} 