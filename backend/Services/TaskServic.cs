using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class TaskService
{
    private readonly AppDbContext _context;

    public TaskService(AppDbContext context)
    {
        _context = context;
    }

    // ✅ CREATE TASK (Admin / Agent)
    public async Task<TaskItem> CreateTaskAsync(string title, string desc, int assignedTo, DateTime dueDate, int? leadId = null, int? dealId = null)
    {
        var task = new TaskItem
        {
            Title = title,
            Description = desc,
            AssignedToUserId = assignedTo,
            DueDate = dueDate,
            LeadId = leadId,
            DealId = dealId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return task;
    }

    // 📊 GET TASKS (filter by user later)
    public async Task<List<TaskItem>> GetTasksAsync()
    {
        return await _context.Tasks
            .Include(t => t.AssignedToUser)
            .Include(t => t.Lead)
            .Include(t => t.Deal)
            .ToListAsync();
    }

    // 🔄 UPDATE STATUS
    public async Task<bool> UpdateStatusAsync(int taskId, string status)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return false;

        task.Status = status;
        await _context.SaveChangesAsync();

        return true;
    }

    // 👤 ASSIGN TASK (Admin)
    public async Task<bool> AssignTaskAsync(int taskId, int agentId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return false;

        task.AssignedToUserId = agentId;
        await _context.SaveChangesAsync();

        return true;
    }
}