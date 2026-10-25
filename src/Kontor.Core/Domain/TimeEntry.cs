using Kontor.Core.Rules;

namespace Kontor.Core.Domain;

/// <summary>
/// A booked working time. Immutable once saved (constitution, principle III):
/// there are no public setters and no update or delete operations.
/// </summary>
public sealed class TimeEntry
{
    public const int NoteMaxLength = 500;

    private TimeEntry()
    {
    }

    public TimeEntry(int employeeId, int projectId, DateOnly date, TimeOnly start, TimeOnly end, string? note)
    {
        EmployeeId = employeeId;
        ProjectId = projectId;
        Date = date;
        Start = start;
        End = end;
        Note = note;
    }

    public int Id { get; private set; }

    public int EmployeeId { get; private set; }

    public int ProjectId { get; private set; }

    public DateOnly Date { get; private set; }

    public TimeOnly Start { get; private set; }

    public TimeOnly End { get; private set; }

    public string? Note { get; private set; }

    /// <summary>The actual duration as booked (FR-004); not stored.</summary>
    public TimeSpan ActualDuration => End - Start;

    /// <summary>The billed duration, rounded up to the next full quarter hour (FR-005); not stored.</summary>
    public TimeSpan BilledDuration => BillingRounding.Round(ActualDuration);
}
