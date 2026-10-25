using Kontor.Core.Rules;

namespace Kontor.Core.Tests.Rules;

[TestClass]
public sealed class BillingRoundingTests
{
    [TestMethod]
    [DataRow(2, 7, 2, 15)]
    [DataRow(2, 0, 2, 0)]
    [DataRow(0, 5, 0, 15)]
    [DataRow(1, 43, 1, 45)]
    [DataRow(0, 1, 0, 15)]
    [DataRow(1, 45, 1, 45)]
    public void Round_RoundsUpToNextFullQuarterHour(int hours, int minutes, int expectedHours, int expectedMinutes)
    {
        var billed = BillingRounding.Round(new TimeSpan(hours, minutes, 0));

        Assert.AreEqual(new TimeSpan(expectedHours, expectedMinutes, 0), billed);
    }
}
