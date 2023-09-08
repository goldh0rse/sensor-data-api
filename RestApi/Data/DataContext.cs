using Microsoft.EntityFrameworkCore;
using RestApi.Models;

namespace RestApi.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Temperature> Temperatures => Set<Temperature>();
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperature>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}