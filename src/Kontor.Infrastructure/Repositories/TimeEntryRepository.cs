using Kontor.Core.Abstractions;
using Kontor.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kontor.Infrastructure.Repositories;

internal sealed class TimeEntryRepository(IDbContextFactory<KontorDbContext> contextFactory) : ITimeEntryRepository
{
    public async Task AddAsync(TimeEntry entry, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        context.TimeEntries.Add(entry);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TimeEntry>> GetForEmployeeAndDateAsync(int employeeId, DateOnly date, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.TimeEntries
            .AsNoTracking()
            .Where(t => t.EmployeeId == employeeId && t.Date == date)
            .OrderBy(t => t.Start)
            .ToListAsync(cancellationToken);
    }
}
