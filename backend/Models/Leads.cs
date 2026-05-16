namespace backend.Models;

public class Lead
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string Phone { get; set; }

    public string Status { get; set; } = "New"; 
    // New, Contacted, Qualified, Converted

    public int? AssignedToUserId { get; set; }
    public User AssignedToUser { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}