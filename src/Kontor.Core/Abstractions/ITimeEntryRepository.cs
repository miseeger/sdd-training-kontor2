using Kontor.Core.Domain;

namespace Kontor.Core.Abstractions;

/// <summary>Stores time entries. Only adding and reading; entries are never changed or deleted.</summary>
public interface ITimeEntryRepository
{
    Task AddAsync(TimeEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TimeEntry>> GetForEmployeeAndDateAsync(int employeeId, DateOnly date, CancellationToken cancellationToken = default);
}
