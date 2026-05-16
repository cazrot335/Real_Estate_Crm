using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class LeadService
{
    private readonly AppDbContext _context;

    public LeadService(AppDbContext context)
    {
        _context = context;
    }

    // CREATE LEAD
    public async Task<Lead> CreateLeadAsync(string name, string phone)
    {
        var lead = new Lead
        {
            Name = name,
            Phone = phone
        };

        _context.Leads.Add(lead);
        await _context.SaveChangesAsync();

        return lead;
    }

    // ASSIGN LEAD (Admin)
    public async Task<bool> AssignLeadAsync(int leadId, int agentId)
    {
        var lead = await _context.Leads.FindAsync(leadId);
        if (lead == null) return false;

        lead.AssignedToUserId = agentId;
        await _context.SaveChangesAsync();

        return true;
    }

    // GET LEADS (role-based filtering later)
    public async Task<List<Lead>> GetLeadsAsync()
    {
        return await _context.Leads
            .Include(l => l.AssignedToUser)
            .ToListAsync();
    }

    // UPDATE STATUS
    public async Task<bool> UpdateStatusAsync(int leadId, string status)
    {
        var lead = await _context.Leads.FindAsync(leadId);
        if (lead == null) return false;

        lead.Status = status;
        await _context.SaveChangesAsync();

        return true;
    }
}