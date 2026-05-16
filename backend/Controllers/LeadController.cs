using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/lead")]
public class LeadController : ControllerBase
{
    private readonly LeadService _service;

    public LeadController(LeadService service)
    {
        _service = service;
    }

    // 🧑‍💼 Agent + Admin → Create Lead
    [Authorize(Policy = "create_lead")]
    [HttpPost]
    public async Task<IActionResult> Create(string name, string phone)
    {
        var lead = await _service.CreateLeadAsync(name, phone);
        return Ok(lead);
    }

    // 👑 Admin → Assign Lead
    [Authorize(Policy = "assign_lead")]
    [HttpPost("assign")]
    public async Task<IActionResult> Assign(int leadId, int agentId)
    {
        var result = await _service.AssignLeadAsync(leadId, agentId);

        if (!result) return NotFound();

        return Ok("Lead assigned");
    }

    // 👀 All → View Leads
    [Authorize(Policy = "view_lead")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetLeadsAsync());
    }

    // 🧑‍💼 Agent → Update status
    [Authorize(Policy = "update_lead_status")]
    [HttpPost("status")]
    public async Task<IActionResult> UpdateStatus(int leadId, string status)
    {
        var result = await _service.UpdateStatusAsync(leadId, status);

        if (!result) return NotFound();

        return Ok("Updated");
    }
}