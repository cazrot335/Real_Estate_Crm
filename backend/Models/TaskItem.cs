namespace backend.Models;

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    // 🔗 Optional relations
    public int? LeadId { get; set; }
    public Lead Lead { get; set; }

    public int? DealId { get; set; }
    public Deal Deal { get; set; }

    // 👤 Assigned agent
    public int AssignedToUserId { get; set; }
    public User AssignedToUser { get; set; }

    public string Status { get; set; } = "Pending"; 
    // Pending, Completed

    public DateTime DueDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}