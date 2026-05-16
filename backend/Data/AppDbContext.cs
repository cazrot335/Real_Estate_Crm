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
    new Permission { Id = 1, Name = "create_agent" },
    new Permission { Id = 2, Name = "view_leads" },
    new Permission { Id = 3, Name = "create_lead" }
);


modelBuilder.Entity<RolePermission>().HasData(
    // Admin → all permissions
    new RolePermission { RoleId = 1, PermissionId = 1 },
    new RolePermission { RoleId = 1, PermissionId = 2 },
    new RolePermission { RoleId = 1, PermissionId = 3 },

    // Agent
    new RolePermission { RoleId = 2, PermissionId = 2 },
    new RolePermission { RoleId = 2, PermissionId = 3 },

    // Viewer
    new RolePermission { RoleId = 3, PermissionId = 2 }
);

           
    }
}