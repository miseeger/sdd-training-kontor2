namespace Kontor.Core.Rules;

/// <summary>End after start, on the same day; a booking over midnight is rejected (FR-006).</summary>
public static class TimeRangeRule
{
    public const string Name = "Zeitraum";

    public static RuleViolation? Check(TimeSlot candidate) =>
        candidate.End > candidate.Start
            ? null
            : new RuleViolation(Name, "Das Ende muss nach dem Beginn liegen, am selben Tag. Wer über Mitternacht arbeitet, bucht zwei Einträge, einen pro Tag.");
}
