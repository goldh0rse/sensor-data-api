using Microsoft.EntityFrameworkCore;
using RestApi.Models;

namespace RestApi.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Temperature> Temperatures => Set<Temperature>();
        public DbSet<Moisture> MoistureLvls => Set<Moisture>();
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperature>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Moisture>()
                .Property(m => m.CreatedAt)
                .HasDefaultValue("CURRENT_TIMESTAMP");
        }
    }
}