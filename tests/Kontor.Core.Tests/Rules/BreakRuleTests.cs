using Kontor.Core.Rules;

namespace Kontor.Core.Tests.Rules;

[TestClass]
public sealed class BreakRuleTests
{
    private static TimeSlot Slot(int startHour, int startMinute, int endHour, int endMinute) =>
        new(new TimeOnly(startHour, startMinute), new TimeOnly(endHour, endMinute));

    [TestMethod]
    public void Check_MoreThanSixHoursWithoutBreak_ReturnsViolation()
    {
        var violation = BreakRule.Check(Slot(7, 0, 14, 0), []);

        Assert.IsNotNull(violation);
        Assert.AreEqual(BreakRule.Name, violation.Rule);
    }

    [TestMethod]
    public void Check_SixAndAHalfHoursWithThirtyMinuteBreak_ReturnsNoViolation()
    {
        var violation = BreakRule.Check(Slot(11, 30, 14, 0), [Slot(7, 0, 11, 0)]);

        Assert.IsNull(violation);
    }

    [TestMethod]
    public void Check_ExactlySixHoursWithoutBreak_ReturnsNoViolation()
    {
        var violation = BreakRule.Check(Slot(13, 0, 14, 0), [Slot(8, 0, 13, 0)]);

        Assert.IsNull(violation);
    }

    [TestMethod]
    public void Check_GapsShorterThanFifteenMinutesDoNotCount()
    {
        // 7 h work, gaps of 10 min each: only gaps of at least 15 min count as a break
        var violation = BreakRule.Check(Slot(12, 20, 14, 20), [Slot(7, 0, 9, 0), Slot(9, 10, 12, 10)]);

        Assert.IsNotNull(violation);
    }

    [TestMethod]
    public void Check_MoreThanNineHoursWithThirtyMinuteBreak_ReturnsViolation()
    {
        var violation = BreakRule.Check(Slot(12, 30, 17, 30), [Slot(7, 0, 12, 0)]);

        Assert.IsNotNull(violation);
    }

    [TestMethod]
    public void Check_MoreThanNineHoursWithFortyFiveMinuteBreak_ReturnsNoViolation()
    {
        var violation = BreakRule.Check(Slot(12, 45, 17, 45), [Slot(7, 0, 12, 0)]);

        Assert.IsNull(violation);
    }
}
