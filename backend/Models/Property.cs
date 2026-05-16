namespace backend.Models;

public class Property
{
    public int Id { get; set; }

    public string Title { get; set; }
    public string Location { get; set; }
    public decimal Price { get; set; }

    public string Status { get; set; } = "Available"; 
    // Available, Sold, Rented

    public int? AssignedAgentId { get; set; }
    public User AssignedAgent { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}