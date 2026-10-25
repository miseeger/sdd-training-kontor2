namespace Kontor.Core.Rules;

/// <summary>Actual working time and counted breaks of one day, including the new booking (FR-013).</summary>
internal static class WorkingDay
{
    public static readonly TimeSpan MinimumCountedBreak = TimeSpan.FromMinutes(15);

    public static TimeSpan WorkingTime(TimeSlot candidate, IReadOnlyCollection<TimeSlot> existing) =>
        existing.Append(candidate).Aggregate(TimeSpan.Zero, (sum, slot) => sum + slot.Duration);

    public static TimeSpan CountedBreaks(TimeSlot candidate, IReadOnlyCollection<TimeSlot> existing)
    {
        var ordered = existing.Append(candidate).OrderBy(slot => slot.Start).ToList();
        var breaks = TimeSpan.Zero;
        for (var i = 1; i < ordered.Count; i++)
        {
            var gap = ordered[i].Start - ordered[i - 1].End;
            if (ordered[i].Start > ordered[i - 1].End && gap >= MinimumCountedBreak)
            {
                breaks += gap;
            }
        }

        return breaks;
    }
}
