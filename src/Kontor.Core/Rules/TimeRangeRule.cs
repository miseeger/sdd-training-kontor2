namespace Kontor.Core.Rules;

public static class TimeRangeRule
{
    public const string Name = "Zeitraum";

    public static RuleViolation? Check(TimeSlot candidate) => throw new NotImplementedException();
}
