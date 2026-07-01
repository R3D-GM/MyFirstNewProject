using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFirstNewProject.Models;

namespace MyFirstNewProject.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ✅ ADD THESE
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<SystemSettings> SystemSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}