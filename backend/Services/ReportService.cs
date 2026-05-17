using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    // 📊 SUMMARY DASHBOARD (Filtered by Role)
    public async Task<object> GetSummaryAsync(string userEmail, string userRole)
    {
        if (userRole == "Admin")
        {
            var totalLeads = await _context.Leads.CountAsync();
            var convertedLeads = await _context.Leads.CountAsync(l => l.Status == "Converted");

            var totalDeals = await _context.Deals.CountAsync();
            var closedDeals = await _context.Deals.CountAsync(d => d.Status == "Closed");

            var totalRevenue = await _context.Deals
                .Where(d => d.Status == "Closed")
                .SumAsync(d => (decimal?)d.Amount) ?? 0;

            return new
            {
                totalLeads,
                convertedLeads,
                totalDeals,
                closedDeals,
                totalRevenue
            };
        }
        else if (userRole == "Agent")
        {
            // Agent sees only their assigned leads and deals (optional limited)
            var totalLeads = await _context.Leads.CountAsync(l => l.AssignedToUser.Email == userEmail);
            var convertedLeads = await _context.Leads.CountAsync(l => l.AssignedToUser.Email == userEmail && l.Status == "Converted");

            var totalDeals = await _context.Deals.CountAsync(d => d.Agent.Email == userEmail);
            var closedDeals = await _context.Deals.CountAsync(d => d.Agent.Email == userEmail && d.Status == "Closed");

            var totalRevenue = await _context.Deals
                .Where(d => d.Agent.Email == userEmail && d.Status == "Closed")
                .SumAsync(d => (decimal?)d.Amount) ?? 0;

            return new
            {
                totalLeads,
                convertedLeads,
                totalDeals,
                closedDeals,
                totalRevenue
            };
        }
        else
        {
            // Viewer sees basic summary with count of leads (only basic)
            var totalLeads = await _context.Leads.CountAsync();
            var convertedLeads = await _context.Leads.CountAsync(l => l.Status == "Converted");

            return new
            {
                totalLeads,
                convertedLeads
            };
        }
    }

    // 🧑‍💼 AGENT PERFORMANCE (Admin only)
    public async Task<object> GetAgentPerformanceAsync()
    {
        var data = await _context.Deals
            .Where(d => d.Status == "Closed")
            .GroupBy(d => d.Agent.Email)
            .Select(g => new
            {
                Agent = g.Key,
                DealsClosed = g.Count(),
                Revenue = g.Sum(d => d.Amount)
            })
            .ToListAsync();

        return data;
    }

    // 🔄 CONVERSION RATE
    public async Task<object> GetConversionRateAsync()
    {
        var totalLeads = await _context.Leads.CountAsync();
        var convertedLeads = await _context.Leads.CountAsync(l => l.Status == "Converted");

        double rate = totalLeads == 0 ? 0 : (double)convertedLeads / totalLeads * 100;

        return new
        {
            totalLeads,
            convertedLeads,
            conversionRate = rate
        };
    }
}