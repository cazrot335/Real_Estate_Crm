using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class DealService
{
    private readonly AppDbContext _context;

    public DealService(AppDbContext context)
    {
        _context = context;
    }

    // 💰 CREATE DEAL (Convert Lead)
    public async Task<(bool Success, string Message, Deal? Deal)> CreateDealAsync(int leadId, int propertyId, int agentId, decimal amount)
    {
        var lead = await _context.Leads.FindAsync(leadId);
        var property = await _context.Properties.FindAsync(propertyId);

        if (lead == null || property == null)
            return (false, "Invalid lead or property", null);

        var deal = new Deal
        {
            LeadId = leadId,
            PropertyId = propertyId,
            AgentId = agentId,
            Amount = amount
        };

        // 🔄 Update lead status
        lead.Status = "Converted";

        _context.Deals.Add(deal);
        await _context.SaveChangesAsync();

        return (true, "Deal created", deal);
    }

    // 📊 GET DEALS
    public async Task<List<Deal>> GetDealsAsync()
    {
        return await _context.Deals
            .Include(d => d.Lead)
            .Include(d => d.Property)
            .Include(d => d.Agent)
            .ToListAsync();
    }

    // 🔄 UPDATE DEAL STATUS
    public async Task<bool> UpdateStatusAsync(int dealId, string status)
    {
        var deal = await _context.Deals.FindAsync(dealId);
        if (deal == null) return false;

        deal.Status = status;
        await _context.SaveChangesAsync();

        return true;
    }
}