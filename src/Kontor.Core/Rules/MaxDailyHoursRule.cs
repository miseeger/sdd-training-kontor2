namespace Kontor.Core.Rules;

public static class MaxDailyHoursRule
{
    public const string Name = "Tageshöchstarbeitszeit";

    public static RuleViolation? Check(TimeSlot candidate, IReadOnlyCollection<TimeSlot> existing) => throw new NotImplementedException();
}
