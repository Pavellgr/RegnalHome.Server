using Microsoft.EntityFrameworkCore;
using RegnalHome.Common.Dtos;
using RegnalHome.Common.Models;

namespace RegnalHome.Server;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<IrrigationModuleDTO> IrrigationModules { get; set; }

    public DbSet<Log> Log { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}