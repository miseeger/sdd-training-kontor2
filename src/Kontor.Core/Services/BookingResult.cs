using Kontor.Core.Domain;
using Kontor.Core.Rules;

namespace Kontor.Core.Services;

public sealed record BookingResult(bool Succeeded, string? ViolatedRule, string? Message, TimeEntry? Entry)
{
    public static BookingResult Saved(TimeEntry entry) => new(true, null, null, entry);

    public static BookingResult Rejected(RuleViolation violation) => new(false, violation.Rule, violation.Message, null);
}
