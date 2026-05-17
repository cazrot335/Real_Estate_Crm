using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/deal")]
public class DealController : ControllerBase
{
    private readonly DealService _service;

    public DealController(DealService service)
    {
        _service = service;
    }

    // 🧑‍💼 Agent + Admin → Create Deal
    [Authorize(Policy = "create_deal")]
    [HttpPost]
    public async Task<IActionResult> Create(int leadId, int propertyId, int agentId, decimal amount)
    {
        var result = await _service.CreateDealAsync(leadId, propertyId, agentId, amount);

        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result.Deal);
    }

    // 👀 All → View Deals
    [Authorize(Policy = "view_deal")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetDealsAsync());
    }

    // 🧑‍💼 Agent + Admin → Update Deal Status
    [Authorize(Policy = "update_deal_status")]
    [HttpPost("status")]
    public async Task<IActionResult> UpdateStatus(int dealId, string status)
    {
        var result = await _service.UpdateStatusAsync(dealId, status);

        if (!result) return NotFound();

        return Ok("Deal updated");
    }
}