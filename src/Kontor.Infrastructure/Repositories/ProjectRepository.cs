using Kontor.Core.Abstractions;
using Kontor.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kontor.Infrastructure.Repositories;

internal sealed class ProjectRepository(IDbContextFactory<KontorDbContext> contextFactory) : IProjectRepository
{
    public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Projects.AsNoTracking().OrderBy(p => p.Key).ToListAsync(cancellationToken);
    }
}
