using Microsoft.EntityFrameworkCore;

namespace RegnalHome.GrpcSim
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Therm> Sensors { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
