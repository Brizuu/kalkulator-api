using Kalkulator_project.Models;
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
    public DbSet<Product> Products { get; set; }
    
    public DbSet<Specification> Specifications { get; set; }
    
}