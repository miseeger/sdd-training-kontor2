using Kontor.Core.Abstractions;

namespace Kontor.Core.Services;

public sealed class TimeEntryService(ITimeEntryRepository repository)
{
    private readonly ITimeEntryRepository _repository = repository;

    public const string NoteRuleName = "Notiz";

    public Task<BookingResult> BookAsync(BookingRequest request, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<DayOverview> GetDayAsync(int employeeId, DateOnly date, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
