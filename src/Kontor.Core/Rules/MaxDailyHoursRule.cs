namespace Kontor.Core.Rules;

/// <summary>The working time of a day must not exceed 10 hours, without exception (FR-009).
/// Uses the actual durations, not the billed ones (FR-013).</summary>
public static class MaxDailyHoursRule
{
    public const string Name = "Tageshöchstarbeitszeit";

    private static readonly TimeSpan Maximum = TimeSpan.FromHours(10);

    public static RuleViolation? Check(TimeSlot candidate, IReadOnlyCollection<TimeSlot> existing)
    {
        var workingTime = WorkingDay.WorkingTime(candidate, existing);
        return workingTime <= Maximum
            ? null
            : new RuleViolation(Name, $"Mit dieser Buchung kämen {(int)workingTime.TotalHours}:{workingTime.Minutes:00} Stunden zusammen; erlaubt sind höchstens 10 Stunden pro Tag.");
    }
}
