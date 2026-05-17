using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Services;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _service;

    public ReportsController(ReportService service)
    {
        _service = service;
    }

    // 📊 Admin → Full dashboard, Agent → Limited, Viewer → Basic
    [Authorize(Policy = "view_reports")]
    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        var userEmail = User.Identity?.Name ?? "";
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Viewer";

        return Ok(await _service.GetSummaryAsync(userEmail, userRole));
    }

    // 🧑‍💼 Admin → Agent performance
    [Authorize(Policy = "view_agent_performance")]
    [HttpGet("agent-performance")]
    public async Task<IActionResult> AgentPerformance()
    {
        return Ok(await _service.GetAgentPerformanceAsync());
    }

    // 👀 All → Conversion rate
    [Authorize(Policy = "view_reports")]
    [HttpGet("conversion-rate")]
    public async Task<IActionResult> ConversionRate()
    {
        return Ok(await _service.GetConversionRateAsync());
    }
}