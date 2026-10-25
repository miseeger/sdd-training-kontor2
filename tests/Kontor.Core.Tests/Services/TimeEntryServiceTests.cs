using Kontor.Core.Rules;
using Kontor.Core.Services;
using Kontor.Core.Tests.Fakes;

namespace Kontor.Core.Tests.Services;

[TestClass]
public sealed class TimeEntryServiceTests
{
    private const int TomasOkoye = 2;
    private const int JunaSato = 3;
    private const int NslAlb01 = 1;
    private const int KbaTg304 = 2;
    private const int HflPer02 = 3;
    private static readonly DateOnly November2 = new(2026, 11, 2);

    private InMemoryTimeEntryRepository _repository = null!;
    private TimeEntryService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _repository = new InMemoryTimeEntryRepository();
        _service = new TimeEntryService(_repository);
    }

    // User Story 1, scenario 1
    [TestMethod]
    public async Task BookAsync_KeepsStartAndEnd_AndBillsRoundedUpDuration()
    {
        var result = await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 10, 10, 17));

        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(new TimeOnly(8, 10), result.Entry!.Start);
        Assert.AreEqual(new TimeOnly(10, 17), result.Entry.End);
        Assert.AreEqual(new TimeSpan(2, 7, 0), result.Entry.ActualDuration);
        Assert.AreEqual(new TimeSpan(2, 15, 0), result.Entry.BilledDuration);
        var day = await _service.GetDayAsync(JunaSato, November2);
        Assert.AreEqual(new TimeSpan(2, 15, 0), day.Total);
    }

    // User Story 1, scenario 2
    [TestMethod]
    public async Task BookAsync_Overlapping_IsRejectedAndNamesRule()
    {
        await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 10, 10, 17));

        var result = await _service.BookAsync(Request(JunaSato, KbaTg304, November2, 10, 0, 12, 0));

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(OverlapRule.Name, result.ViolatedRule);
        Assert.HasCount(1, _repository.Entries);
    }

    // User Story 1, scenario 3; FR-012, SC-003: the daily total is the sum of the billed durations
    [TestMethod]
    public async Task BookAsync_Touching_IsSaved_AndTotalIsSumOfBilledDurations()
    {
        await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 10, 10, 17));

        var result = await _service.BookAsync(Request(JunaSato, KbaTg304, November2, 10, 17, 12, 0));

        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(new TimeSpan(1, 45, 0), result.Entry!.BilledDuration);
        var day = await _service.GetDayAsync(JunaSato, November2);
        Assert.HasCount(2, day.Entries);
        Assert.AreEqual(new TimeSpan(4, 0, 0), day.Total);
    }

    // User Story 1, scenario 4
    [TestMethod]
    public async Task BookAsync_FullQuarterHours_AreNotRoundedFurther()
    {
        var result = await _service.BookAsync(Request(TomasOkoye, HflPer02, November2, 8, 0, 10, 0));

        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(new TimeSpan(2, 0, 0), result.Entry!.ActualDuration);
        Assert.AreEqual(new TimeSpan(2, 0, 0), result.Entry.BilledDuration);
    }

    // FR-007: only bookings of the same person count
    [TestMethod]
    public async Task BookAsync_SameTimeOtherPerson_IsSaved()
    {
        await _service.BookAsync(Request(TomasOkoye, HflPer02, November2, 8, 0, 10, 0));

        var result = await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 10, 10, 17));

        Assert.IsTrue(result.Succeeded);
    }

    // FR-006, FR-010
    [TestMethod]
    public async Task BookAsync_EndBeforeStart_IsRejectedAndNamesRule()
    {
        var result = await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 10, 0, 9, 0));

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(TimeRangeRule.Name, result.ViolatedRule);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Message));
    }

    // data-model TimeEntry.Note: optional, max. 500 Zeichen
    [TestMethod]
    public async Task BookAsync_NoteLongerThan500Characters_IsRejected()
    {
        var result = await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 0, 12, 0, new string('x', 501)));

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(TimeEntryService.NoteRuleName, result.ViolatedRule);
        Assert.HasCount(0, _repository.Entries);
    }

    [TestMethod]
    public async Task BookAsync_NoteWith500Characters_IsSaved()
    {
        var result = await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 0, 12, 0, new string('x', 500)));

        Assert.IsTrue(result.Succeeded);
    }

    private const int RikHalvorsen = 4;
    private static readonly DateOnly November3 = new(2026, 11, 3);

    // User Story 2, scenario 1
    [TestMethod]
    public async Task BookAsync_SevenHoursWithoutBreak_IsRejected()
    {
        var result = await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 7, 0, 14, 0));

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(BreakRule.Name, result.ViolatedRule);
    }

    // User Story 2, scenario 2
    [TestMethod]
    public async Task BookAsync_WithThirtyMinuteBreak_IsSaved()
    {
        await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 7, 0, 11, 0));

        var result = await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 11, 30, 14, 0));

        Assert.IsTrue(result.Succeeded);
    }

    // User Story 2, scenario 3: only the new booking is rejected, existing ones stay
    [TestMethod]
    public async Task BookAsync_ExceedingTenHours_IsRejectedAndExistingEntriesStay()
    {
        await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 7, 0, 11, 0));
        await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 11, 30, 14, 0));
        await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 14, 45, 17, 15));

        var result = await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 17, 30, 19, 0));

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(MaxDailyHoursRule.Name, result.ViolatedRule);
        Assert.HasCount(3, _repository.Entries);
    }

    // FR-013: the rules use the actual durations, the daily total the billed ones
    [TestMethod]
    public async Task BookAsync_TenActualHours_IsSavedAlthoughBilledTotalIsHigher()
    {
        await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 6, 0, 9, 23));
        await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 10, 0, 13, 23));

        var result = await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 14, 0, 17, 14));

        Assert.IsTrue(result.Succeeded);
        var day = await _service.GetDayAsync(RikHalvorsen, November3);
        Assert.AreEqual(new TimeSpan(10, 15, 0), day.Total);
    }

    internal static BookingRequest Request(int employeeId, int projectId, DateOnly date, int startHour, int startMinute, int endHour, int endMinute, string? note = null) =>
        new(employeeId, projectId, date, new TimeOnly(startHour, startMinute), new TimeOnly(endHour, endMinute), note);
}
