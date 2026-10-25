using Kontor.Core.Abstractions;
using Kontor.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kontor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddKontorInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextFactory<KontorDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        return services;
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var factory = services.GetRequiredService<IDbContextFactory<KontorDbContext>>();
        await using var context = await factory.CreateDbContextAsync(cancellationToken);
        await context.Database.MigrateAsync(cancellationToken);
    }
}
