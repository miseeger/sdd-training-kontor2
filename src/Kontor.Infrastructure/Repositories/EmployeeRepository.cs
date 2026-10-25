using Kontor.Core.Abstractions;
using Kontor.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kontor.Infrastructure.Repositories;

internal sealed class EmployeeRepository(IDbContextFactory<KontorDbContext> contextFactory) : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Employees.AsNoTracking().OrderBy(e => e.Name).ToListAsync(cancellationToken);
    }
}
