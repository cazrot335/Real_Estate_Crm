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

    var token = _authService.GenerateJwtToken(result.User);

    return Ok(new
    {
        token,
        role = result.User.Role
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

[HttpPost("create-agent")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> CreateAgent(CreateAgentRequest request)
{
    var result = await _authService.CreateAgentAsync(request);

    if (!result.Success)
        return BadRequest(result.Message);

    return Ok(result.Message);
}
}
