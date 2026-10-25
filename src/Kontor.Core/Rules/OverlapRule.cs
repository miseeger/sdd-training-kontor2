namespace Kontor.Core.Rules;

/// <summary>No overlap with other bookings of the same person on that day; touching is allowed (FR-007).</summary>
public static class OverlapRule
{
    public const string Name = "Überschneidung";

    public static RuleViolation? Check(TimeSlot candidate, IReadOnlyCollection<TimeSlot> existing)
    {
        foreach (var slot in existing)
        {
            if (candidate.Start < slot.End && slot.Start < candidate.End)
            {
                return new RuleViolation(Name, $"Die Buchung überschneidet sich mit {slot.Start:HH\\:mm}–{slot.End:HH\\:mm}.");
            }
        }

        return null;
    }
}
