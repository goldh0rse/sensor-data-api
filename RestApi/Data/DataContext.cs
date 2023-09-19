using Microsoft.EntityFrameworkCore;
using RestApi.Models;

namespace RestApi.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Temperature> Temperatures => Set<Temperature>();
        public DbSet<Soil> SoilReadings => Set<Soil>();
        public DbSet<Light> LightReadings => Set<Light>();
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Temperature>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Soil>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Light>()
                .Property(l => l.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}