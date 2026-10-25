namespace Kontor.Core.Rules;

/// <summary>
/// Billed duration: the actual duration rounded up to the next full service quarter hour (FR-005).
/// A duration that is already a multiple of 15 minutes stays unchanged. Start and end are never rounded.
/// </summary>
public static class BillingRounding
{
    public static readonly TimeSpan QuarterHour = TimeSpan.FromMinutes(15);

    public static TimeSpan Round(TimeSpan actualDuration)
    {
        var quarters = (actualDuration.Ticks + QuarterHour.Ticks - 1) / QuarterHour.Ticks;
        return TimeSpan.FromTicks(quarters * QuarterHour.Ticks);
    }
}
