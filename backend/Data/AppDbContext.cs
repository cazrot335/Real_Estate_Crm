using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Lead> Leads { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Deal> Deals { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        // new RolePermission key
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });
    
 

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Agent" },
            new Role { Id = 3, Name = "Viewer" }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1, Name = "create_lead" },
            new Permission { Id = 2, Name = "assign_lead" },
            new Permission { Id = 3, Name = "view_lead" },
            new Permission { Id = 4, Name = "update_lead_status" },
            new Permission { Id = 5, Name = "create_property" },
            new Permission { Id = 6, Name = "assign_property" },
            new Permission { Id = 7, Name = "view_property" },
            new Permission { Id = 8, Name = "update_property_status" },
            new Permission { Id = 9, Name = "create_deal" },
            new Permission { Id = 10, Name = "view_deal" },
            new Permission { Id = 11, Name = "update_deal_status" },
            new Permission { Id = 12, Name = "create_task" },
            new Permission { Id = 13, Name = "view_task" },
            new Permission { Id = 14, Name = "update_task_status" },
            new Permission { Id = 15, Name = "assign_task" },
            new Permission { Id = 16, Name = "view_reports" },
            new Permission { Id = 17, Name = "view_agent_performance" }
        );


        modelBuilder.Entity<RolePermission>().HasData(
            // Admin → ALL
            new RolePermission { RoleId = 1, PermissionId = 1 },
            new RolePermission { RoleId = 1, PermissionId = 2 },
            new RolePermission { RoleId = 1, PermissionId = 3 },
            new RolePermission { RoleId = 1, PermissionId = 4 },
            new RolePermission { RoleId = 1, PermissionId = 5 },
            new RolePermission { RoleId = 1, PermissionId = 6 },
            new RolePermission { RoleId = 1, PermissionId = 7 },
            new RolePermission { RoleId = 1, PermissionId = 8 },
            new RolePermission { RoleId = 1, PermissionId = 9 },
            new RolePermission { RoleId = 1, PermissionId = 10 },
            new RolePermission { RoleId = 1, PermissionId = 11 },
            new RolePermission { RoleId = 1, PermissionId = 12 },
            new RolePermission { RoleId = 1, PermissionId = 13 },
            new RolePermission { RoleId = 1, PermissionId = 14 },
            new RolePermission { RoleId = 1, PermissionId = 15 },
            new RolePermission { RoleId = 1, PermissionId = 16 },
            new RolePermission { RoleId = 1, PermissionId = 17 },

            // Agent → lead + property + deal + task + report permissions
            new RolePermission { RoleId = 2, PermissionId = 1 },
            new RolePermission { RoleId = 2, PermissionId = 3 },
            new RolePermission { RoleId = 2, PermissionId = 4 },
            new RolePermission { RoleId = 2, PermissionId = 5 },
            new RolePermission { RoleId = 2, PermissionId = 7 },
            new RolePermission { RoleId = 2, PermissionId = 8 },
            new RolePermission { RoleId = 2, PermissionId = 9 },
            new RolePermission { RoleId = 2, PermissionId = 10 },
            new RolePermission { RoleId = 2, PermissionId = 11 },
            new RolePermission { RoleId = 2, PermissionId = 12 },
            new RolePermission { RoleId = 2, PermissionId = 13 },
            new RolePermission { RoleId = 2, PermissionId = 14 },
            new RolePermission { RoleId = 2, PermissionId = 16 },

            // Viewer → view permissions
            new RolePermission { RoleId = 3, PermissionId = 3 },
            new RolePermission { RoleId = 3, PermissionId = 7 },
            new RolePermission { RoleId = 3, PermissionId = 10 },
            new RolePermission { RoleId = 3, PermissionId = 13 },
            new RolePermission { RoleId = 3, PermissionId = 16 }
        );
    }
}