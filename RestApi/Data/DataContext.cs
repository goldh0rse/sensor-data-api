using Microsoft.EntityFrameworkCore;
using rest_api.Models;

namespace rest_api.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Character> Characters => Set<Character>();
        public virtual DbSet<Temperature> Temperatures => Set<Temperature>();
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

    }
}