using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/task")]
public class TaskController : ControllerBase
{
    private readonly TaskService _service;

    public TaskController(TaskService service)
    {
        _service = service;
    }

    // 🧑‍💼 Admin + Agent → Create Task
    [Authorize(Policy = "create_task")]
    [HttpPost]
    public async Task<IActionResult> Create(string title, string desc, int assignedTo, DateTime dueDate, int? leadId, int? dealId)
    {
        var task = await _service.CreateTaskAsync(title, desc, assignedTo, dueDate, leadId, dealId);
        return Ok(task);
    }

    // 👑 Admin → Assign Task
    [Authorize(Policy = "assign_task")]
    [HttpPost("assign")]
    public async Task<IActionResult> Assign(int taskId, int agentId)
    {
        var result = await _service.AssignTaskAsync(taskId, agentId);

        if (!result) return NotFound();

        return Ok("Task assigned");
    }

    // 👀 All → View Tasks
    [Authorize(Policy = "view_task")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetTasksAsync());
    }

    // 🧑‍💼 Agent + Admin → Mark Complete
    [Authorize(Policy = "update_task_status")]
    [HttpPost("status")]
    public async Task<IActionResult> UpdateStatus(int taskId, string status)
    {
        var result = await _service.UpdateStatusAsync(taskId, status);

        if (!result) return NotFound();

        return Ok("Task updated");
    }
}