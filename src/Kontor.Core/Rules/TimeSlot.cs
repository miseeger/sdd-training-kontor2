namespace Kontor.Core.Rules;

/// <summary>Start and end of a booking within one day.</summary>
public readonly record struct TimeSlot(TimeOnly Start, TimeOnly End)
{
    public TimeSpan Duration => End > Start ? End - Start : TimeSpan.Zero;
}
