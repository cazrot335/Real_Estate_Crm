using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/property")]
public class PropertyController : ControllerBase
{
    private readonly PropertyService _service;

    public PropertyController(PropertyService service)
    {
        _service = service;
    }

    // 🧑‍💼 Agent + Admin → Create Property
    [Authorize(Policy = "create_property")]
    [HttpPost]
    public async Task<IActionResult> Create(string title, string location, decimal price)
    {
        var property = await _service.CreatePropertyAsync(title, location, price);
        return Ok(property);
    }

    // 👑 Admin → Assign Property
    [Authorize(Policy = "assign_property")]
    [HttpPost("assign")]
    public async Task<IActionResult> Assign(int propertyId, int agentId)
    {
        var result = await _service.AssignPropertyAsync(propertyId, agentId);

        if (!result) return NotFound();

        return Ok("Property assigned");
    }

    // 👀 All → View Properties
    [Authorize(Policy = "view_property")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetPropertiesAsync());
    }

    // 🧑‍💼 Agent + Admin → Update status
    [Authorize(Policy = "update_property_status")]
    [HttpPost("status")]
    public async Task<IActionResult> UpdateStatus(int propertyId, string status)
    {
        var result = await _service.UpdateStatusAsync(propertyId, status);

        if (!result) return NotFound();

        return Ok("Updated");
    }
}