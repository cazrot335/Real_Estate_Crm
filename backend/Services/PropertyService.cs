using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class PropertyService
{
    private readonly AppDbContext _context;

    public PropertyService(AppDbContext context)
    {
        _context = context;
    }

    // CREATE PROPERTY (Admin / Agent)
    public async Task<Property> CreatePropertyAsync(string title, string location, decimal price)
    {
        var property = new Property
        {
            Title = title,
            Location = location,
            Price = price
        };

        _context.Properties.Add(property);
        await _context.SaveChangesAsync();

        return property;
    }

    // ASSIGN PROPERTY TO AGENT (Admin)
    public async Task<bool> AssignPropertyAsync(int propertyId, int agentId)
    {
        var property = await _context.Properties.FindAsync(propertyId);
        if (property == null) return false;

        property.AssignedAgentId = agentId;
        await _context.SaveChangesAsync();

        return true;
    }

    // GET PROPERTIES
    public async Task<List<Property>> GetPropertiesAsync()
    {
        return await _context.Properties
            .Include(p => p.AssignedAgent)
            .ToListAsync();
    }

    // UPDATE PROPERTY STATUS
    public async Task<bool> UpdateStatusAsync(int propertyId, string status)
    {
        var property = await _context.Properties.FindAsync(propertyId);
        if (property == null) return false;

        property.Status = status;
        await _context.SaveChangesAsync();

        return true;
    }
}