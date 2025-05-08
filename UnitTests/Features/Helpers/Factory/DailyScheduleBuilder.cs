using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace UnitTests.Features.Helpers.Factory;

public class DailyScheduleBuilder
{
    private DateOnly _scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
    private TimeOnly _availableFrom = new TimeOnly(10, 00);
    private TimeOnly _availableUntil = new TimeOnly(16, 00);
    private readonly List<(TimeOnly start, TimeOnly end)> _vipTimeRanges = new();
    private bool _activate;
    private ScheduleId _scheduleId = ScheduleId.Create();

    private readonly List<Court> _courts = new();

    private IDateProvider _dateProvider;
    private IScheduleFinder _scheduleFinder;

    private DailyScheduleBuilder(IDateProvider dateProvider, IScheduleFinder scheduleFinder)
    {
        _dateProvider = dateProvider;
        _scheduleFinder = scheduleFinder;
    }

    public static DailyScheduleBuilder CreateValid()
    {
        var repo = new FakeDailyScheduleRepository();
        var dateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var finder = new FakeScheduleFinder(repo);

        return new DailyScheduleBuilder(dateProvider, finder);
    }

    public DailyScheduleBuilder WithScheduleId(ScheduleId id)
    {
        _scheduleId = id;
        return this;
    }

    public DailyScheduleBuilder WithDate(DateOnly date)
    {
        _scheduleDate = date;
        return this;
    }

    public DailyScheduleBuilder WithTimeRange(TimeOnly from, TimeOnly until)
    {
        _availableFrom = from;
        _availableUntil = until;
        return this;
    }

    public DailyScheduleBuilder WithVipTime(TimeOnly start, TimeOnly end)
    {
        _vipTimeRanges.Add((start, end));
        return this;
    }

    public DailyScheduleBuilder WithCourt(Court court)
    {
        _courts.Add(court);
        return this;
    }

    public DailyScheduleBuilder WithDateProvider(IDateProvider provider)
    {
        _dateProvider = provider;
        return this;
    }

    public DailyScheduleBuilder WithScheduleFinder(IScheduleFinder finder)
    {
        _scheduleFinder = finder;
        return this;
    }

    public DailyScheduleBuilder Activate()
    {
        _activate = true;
        return this;
    }

    public Result<DailySchedule> BuildAsync()
    {
        var result = DailySchedule.CreateSchedule(_dateProvider, _scheduleId);
        if (!result.Success) return Result<DailySchedule>.Fail(result.ErrorMessage);

        var schedule = result.Data;

        foreach (var court in _courts)
        {
            schedule.listOfCourts.Add(court);
            schedule.AddAvailableCourt(court, _dateProvider, _scheduleFinder);
        }

        var updateResult = schedule.UpdateScheduleDateAndTime(
            _scheduleDate,
            _availableFrom,
            _availableUntil,
            _dateProvider
        );

        if (!updateResult.Success)
            return Result<DailySchedule>.Fail(updateResult.ErrorMessage);

        if (_activate)
            schedule.Activate(_dateProvider);

        foreach (var vip in _vipTimeRanges)
        {
            var range = VipTimeRange.Create(vip.start, vip.end).Data;
            var vipResult = schedule.AddVipTimeSlots(range);
            if (!vipResult.Success)
                return Result<DailySchedule>.Fail(vipResult.ErrorMessage);
        }

        return Result<DailySchedule>.Ok(schedule);
    }
}