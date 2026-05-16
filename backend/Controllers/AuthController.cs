using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
public async Task<IActionResult> Login(LoginRequest request)
{
    var result = await _authService.LoginAsync(request);

    if (!result.Success)
        return Unauthorized(result.Message);

    return Ok(new
    {
        token = result.Token,
        message = result.Message
    });
}

    [HttpPost("register")]
public async Task<IActionResult> Register(RegisterRequest request)
{
    var result = await _authService.RegisterUserAsync(request);

    if (!result.Success)
        return BadRequest(result.Message);

    return Ok(result.Message);
}

[Authorize(Roles = "Admin")]
[HttpPost("create-agent")]
public async Task<IActionResult> CreateAgent(CreateAgentRequest request)
{
    var result = await _authService.CreateAgentAsync(request);

    if (!result.Success)
        return BadRequest(result.Message);

    return Ok(result.Message);
}

[Authorize(Roles = "Admin,Agent")]
[HttpGet("leads")]
public IActionResult GetLeads()
{
    return Ok("Leads data");
}

[Authorize]
[HttpGet("profile")]
public IActionResult GetProfile()
{
    return Ok("User profile");
}
}
