using Kontor.Core.Abstractions;
using Kontor.Core.Domain;
using Kontor.Core.Rules;

namespace Kontor.Core.Services;

/// <summary>
/// Books working time: checks the rules with the actual times against the day of the person and saves
/// start and end as entered (FR-004). Only the billed duration is rounded (FR-005, FR-012).
/// </summary>
public sealed class TimeEntryService(ITimeEntryRepository repository)
{
    public const string NoteRuleName = "Notiz";

    public async Task<BookingResult> BookAsync(BookingRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Note is { Length: > TimeEntry.NoteMaxLength })
        {
            return BookingResult.Rejected(new RuleViolation(NoteRuleName, $"Die Notiz darf höchstens {TimeEntry.NoteMaxLength} Zeichen lang sein."));
        }

        var candidate = new TimeSlot(request.Start, request.End);
        var existing = (await repository.GetForEmployeeAndDateAsync(request.EmployeeId, request.Date, cancellationToken))
            .Select(e => new TimeSlot(e.Start, e.End))
            .ToList();

        var violation = TimeRangeRule.Check(candidate)
            ?? OverlapRule.Check(candidate, existing)
            ?? BreakRule.Check(candidate, existing)
            ?? MaxDailyHoursRule.Check(candidate, existing);
        if (violation is not null)
        {
            return BookingResult.Rejected(violation);
        }

        var entry = new TimeEntry(request.EmployeeId, request.ProjectId, request.Date, candidate.Start, candidate.End, request.Note);
        await repository.AddAsync(entry, cancellationToken);
        return BookingResult.Saved(entry);
    }

    public async Task<DayOverview> GetDayAsync(int employeeId, DateOnly date, CancellationToken cancellationToken = default)
    {
        var entries = await repository.GetForEmployeeAndDateAsync(employeeId, date, cancellationToken);
        var total = entries.Aggregate(TimeSpan.Zero, (sum, e) => sum + e.BilledDuration);
        return new DayOverview(entries, total);
    }
}
