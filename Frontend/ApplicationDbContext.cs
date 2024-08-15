using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using WebApplication6.Models;

namespace WebApplication6.Models
{
    public class ApplicationDbContext : DbContext
    {
        
        public DbSet<Driver> Drivers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<WebApplication6.Models.Bus> BusTask { get; set; } = default!;
    }
}
