using Microsoft.EntityFrameworkCore;
using PlanetEvaluateApi.Models;

namespace PlanetEvaluateApi.Data
{
    public class PlanetEvaluateDbContext : DbContext
    {
        public PlanetEvaluateDbContext(DbContextOptions<PlanetEvaluateDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserName).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
