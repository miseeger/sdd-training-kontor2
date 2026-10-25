using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kontor.Infrastructure;

/// <summary>Used by the dotnet-ef tool to create migrations without starting the web app.</summary>
public sealed class KontorDbContextFactory : IDesignTimeDbContextFactory<KontorDbContext>
{
    public KontorDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<KontorDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=KontorDb;Trusted_Connection=True")
            .Options;
        return new KontorDbContext(options);
    }
}
