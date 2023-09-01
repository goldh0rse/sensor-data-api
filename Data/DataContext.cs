using Microsoft.EntityFrameworkCore;
using rest_api.Models;

namespace rest_api.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Character> Characters => Set<Character>();

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //   optionsBuilder.UseNpgsql("Host=localhost;Database=rpg-api-db;Username=goldh0rse;Password=temp-password");
        // }

    }
}