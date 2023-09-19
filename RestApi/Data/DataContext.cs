using Microsoft.EntityFrameworkCore;
using RestApi.Models;

namespace RestApi.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Temperature> Temperatures => Set<Temperature>();
        public DbSet<Soil> SoilReadings => Set<Soil>();
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperature>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Soil>()
                .Property(m => m.CreatedAt)
                .HasDefaultValue("CURRENT_TIMESTAMP");
        }
    }
}