using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Tools.OperationResult;

namespace UnitTests.Features.Helpers.Factory;

public class DailyScheduleBuilder
{
    private DateOnly? _scheduleDate;
    private TimeOnly? _availableFrom;
    private TimeOnly? _availableUntil;
    private readonly List<(TimeOnly start, TimeOnly end)> _vipTimeRanges = new();
    private bool _activate = false;
    
    private List<Court> _courts = new();

    private IDateProvider _dateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
    private IScheduleFinder _scheduleFinder = new FakeScheduleFinder();

    private DailyScheduleBuilder()
    {
    }

    public static DailyScheduleBuilder CreateValid() => new();

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
        var result = DailySchedule.CreateSchedule(_dateProvider);
        if (!result.Success)
        {
            return Result<DailySchedule>.Fail(result.ErrorMessage);
        }

        var schedule = result.Data;

        if (_courts.Any())
        {
            foreach (var court in _courts)
            {
                schedule.listOfCourts.Add(court);
                schedule.AddAvailableCourt(court, _dateProvider, _scheduleFinder);
            }
        }
        
        if (_scheduleDate != null || _availableFrom != null || _availableUntil != null)
        {
            var updateResult = schedule.UpdateScheduleDateAndTime(
                _scheduleDate ?? _dateProvider.Today(),
                _availableFrom ?? new TimeOnly(15, 0),
                _availableUntil ?? new TimeOnly(22, 0),
                _dateProvider
            );

            if (!updateResult.Success)
                return Result<DailySchedule>.Fail(updateResult.ErrorMessage);
        }

        if (_activate)
        {
            schedule.Activate(_dateProvider);
        }

        foreach (var vip in _vipTimeRanges)
        {
            var vipResult = schedule.AddVipTimeSlots(vip.start, vip.end);
            if (!vipResult.Success)
                return Result<DailySchedule>.Fail(vipResult.ErrorMessage);
        }

        return Result<DailySchedule>.Ok(schedule);
    }
}