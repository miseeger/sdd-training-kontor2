using Kontor.Core.Rules;

namespace Kontor.Core.Tests.Rules;

[TestClass]
public sealed class MaxDailyHoursRuleTests
{
    private static TimeSlot Slot(int startHour, int endHour) => new(new TimeOnly(startHour, 0), new TimeOnly(endHour, 0));

    [TestMethod]
    public void Check_MoreThanTenHours_ReturnsViolation()
    {
        var violation = MaxDailyHoursRule.Check(Slot(13, 19), [Slot(7, 12)]);

        Assert.IsNotNull(violation);
        Assert.AreEqual(MaxDailyHoursRule.Name, violation.Rule);
    }

    [TestMethod]
    public void Check_ExactlyTenHours_ReturnsNoViolation()
    {
        var violation = MaxDailyHoursRule.Check(Slot(13, 18), [Slot(7, 12)]);

        Assert.IsNull(violation);
    }

    // FR-013: actual 10:00 h is allowed, although the billed durations add up to 10:15 h
    [TestMethod]
    public void Check_TenActualHoursFromThreeEntries_ReturnsNoViolation()
    {
        TimeSlot slot(int sh, int sm, int eh, int em) => new(new TimeOnly(sh, sm), new TimeOnly(eh, em));

        var violation = MaxDailyHoursRule.Check(slot(14, 0, 17, 14), [slot(6, 0, 9, 23), slot(10, 0, 13, 23)]);

        Assert.IsNull(violation);
    }
}
