using Microsoft.EntityFrameworkCore;

namespace Kalkulator_project.Entities;

public class AppDbContext :DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<Services> Services { get; set; }
    public DbSet<UserServices> UserServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}