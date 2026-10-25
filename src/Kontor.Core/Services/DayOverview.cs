using Kontor.Core.Domain;

namespace Kontor.Core.Services;

/// <summary>All bookings of a person on one day and the daily total, the sum of the billed durations (FR-012).</summary>
public sealed record DayOverview(IReadOnlyList<TimeEntry> Entries, TimeSpan Total);
