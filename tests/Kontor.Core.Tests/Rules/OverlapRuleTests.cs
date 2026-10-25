using Kontor.Core.Rules;

namespace Kontor.Core.Tests.Rules;

[TestClass]
public sealed class OverlapRuleTests
{
    private static readonly TimeSlot Morning = new(new TimeOnly(8, 10), new TimeOnly(10, 17));

    [TestMethod]
    public void Check_Overlapping_ReturnsViolation()
    {
        var violation = OverlapRule.Check(new TimeSlot(new TimeOnly(10, 0), new TimeOnly(12, 0)), [Morning]);

        Assert.IsNotNull(violation);
        Assert.AreEqual(OverlapRule.Name, violation.Rule);
        StringAssert.Contains(violation.Message, "08:10");
    }

    [TestMethod]
    public void Check_Touching_ReturnsNoViolation()
    {
        var violation = OverlapRule.Check(new TimeSlot(new TimeOnly(10, 17), new TimeOnly(12, 0)), [Morning]);

        Assert.IsNull(violation);
    }

    [TestMethod]
    public void Check_NoExistingEntries_ReturnsNoViolation()
    {
        var violation = OverlapRule.Check(Morning, []);

        Assert.IsNull(violation);
    }
}
