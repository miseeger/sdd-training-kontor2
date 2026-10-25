using Kontor.Core.Rules;

namespace Kontor.Core.Tests.Rules;

[TestClass]
public sealed class TimeRangeRuleTests
{
    [TestMethod]
    public void Check_EndAfterStart_ReturnsNoViolation()
    {
        var violation = TimeRangeRule.Check(new TimeSlot(new TimeOnly(8, 10), new TimeOnly(10, 17)));

        Assert.IsNull(violation);
    }

    [TestMethod]
    [DataRow(10, 0, 9, 0)]
    [DataRow(8, 0, 8, 0)]
    [DataRow(22, 0, 0, 30)]
    public void Check_EndNotAfterStartOrAfterMidnight_ReturnsViolation(int startHour, int startMinute, int endHour, int endMinute)
    {
        var violation = TimeRangeRule.Check(new TimeSlot(new TimeOnly(startHour, startMinute), new TimeOnly(endHour, endMinute)));

        Assert.IsNotNull(violation);
        Assert.AreEqual(TimeRangeRule.Name, violation.Rule);
    }

    [TestMethod]
    public void Check_FiveMinutes_ReturnsNoViolation()
    {
        var violation = TimeRangeRule.Check(new TimeSlot(new TimeOnly(8, 5), new TimeOnly(8, 10)));

        Assert.IsNull(violation);
    }
}
