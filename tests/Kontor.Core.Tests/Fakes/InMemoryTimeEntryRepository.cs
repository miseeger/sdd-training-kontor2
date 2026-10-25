using Kontor.Core.Abstractions;
using Kontor.Core.Domain;

namespace Kontor.Core.Tests.Fakes;

/// <summary>Hand-written test double (no mocking library, constitution principle II).</summary>
internal sealed class InMemoryTimeEntryRepository : ITimeEntryRepository
{
    private readonly List<TimeEntry> _entries = [];

    public IReadOnlyList<TimeEntry> Entries => _entries;

    public Task AddAsync(TimeEntry entry, CancellationToken cancellationToken = default)
    {
        _entries.Add(entry);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TimeEntry>> GetForEmployeeAndDateAsync(int employeeId, DateOnly date, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TimeEntry> result = _entries
            .Where(t => t.EmployeeId == employeeId && t.Date == date)
            .OrderBy(t => t.Start)
            .ToList();
        return Task.FromResult(result);
    }
}
