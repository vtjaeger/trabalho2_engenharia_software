using Microsoft.EntityFrameworkCore;
using trabalho2.Domain;

namespace trabalho2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<UserLog> UserLogs { get; set; }
    }
}