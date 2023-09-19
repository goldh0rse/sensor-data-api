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
    }
}