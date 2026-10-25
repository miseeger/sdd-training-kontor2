namespace Kontor.Core.Rules;

public static class BreakRule
{
    public const string Name = "Pause";

    public static RuleViolation? Check(TimeSlot candidate, IReadOnlyCollection<TimeSlot> existing) => throw new NotImplementedException();
}
