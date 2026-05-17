

namespace backend.Models;
public class Deal
{
    public int Id { get; set; }

    // 🔗 Relations
    public int LeadId { get; set; }
    public Lead Lead { get; set; }

    public int PropertyId { get; set; }
    public Property Property { get; set; }

    public int AgentId { get; set; }
    public User Agent { get; set; }

    // 💰 Business Data
    public decimal Amount { get; set; }

    public string Status { get; set; } = "Open"; 
    // Open, Negotiation, Closed, Cancelled

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}