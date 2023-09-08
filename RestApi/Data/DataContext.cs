using Microsoft.EntityFrameworkCore;
using RestApi.Models;

namespace RestApi.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Character> Characters => Set<Character>();
        public DbSet<Temperature> Temperatures => Set<Temperature>();
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

    }
}