using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AuthService _authService;

    public AdminController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(AssignRoleRequest request)
    {
        var result = await _authService.AssignRoleToUserAsync(request);

        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }

    [HttpPost("assign-permission")]
    public async Task<IActionResult> AssignPermission(AssignPermissionRequest request)
    {
        var result = await _authService.AssignPermissionToRoleAsync(request);

        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _authService.GetUsersAsync());
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        return Ok(await _authService.GetRolesAsync());
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions()
    {
        return Ok(await _authService.GetPermissionsAsync());
    }
}