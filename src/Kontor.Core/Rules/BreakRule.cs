namespace Kontor.Core.Rules;

/// <summary>
/// More than 6 hours of work need at least 30 minutes of break, more than 9 hours at least 45 minutes.
/// Only gaps of at least 15 minutes between two bookings count as a break (FR-008).
/// Uses the actual durations, not the billed ones (FR-013).
/// </summary>
public static class BreakRule
{
    public const string Name = "Pause";

    public static RuleViolation? Check(TimeSlot candidate, IReadOnlyCollection<TimeSlot> existing)
    {
        var workingTime = WorkingDay.WorkingTime(candidate, existing);
        var breaks = WorkingDay.CountedBreaks(candidate, existing);

        var required = workingTime > TimeSpan.FromHours(9) ? TimeSpan.FromMinutes(45)
            : workingTime > TimeSpan.FromHours(6) ? TimeSpan.FromMinutes(30)
            : TimeSpan.Zero;

        return breaks >= required
            ? null
            : new RuleViolation(Name, $"Bei {Format(workingTime)} Stunden Arbeitszeit sind mindestens {required.TotalMinutes:0} Minuten Pause nötig, erfasst sind {breaks.TotalMinutes:0} Minuten. Als Pause zählen nur Lücken ab 15 Minuten.");
    }

    private static string Format(TimeSpan duration) => $"{(int)duration.TotalHours}:{duration.Minutes:00}";
}
