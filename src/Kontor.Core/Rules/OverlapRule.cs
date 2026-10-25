namespace Kontor.Core.Rules;

public static class OverlapRule
{
    public const string Name = "Überschneidung";

    public static RuleViolation? Check(TimeSlot candidate, IReadOnlyCollection<TimeSlot> existing) => throw new NotImplementedException();
}
