using Kontor.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kontor.Infrastructure;

public sealed class KontorDbContext(DbContextOptions<KontorDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KontorDbContext).Assembly);
    }
}
