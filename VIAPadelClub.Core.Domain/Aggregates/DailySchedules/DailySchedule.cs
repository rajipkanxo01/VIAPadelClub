using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Common.BaseClasses;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

public class DailySchedule : AggregateRoot
{
    public Guid scheduleId;
    internal DateTime scheduleDate;
    internal TimeSpan availableFrom;
    internal TimeSpan availableUntil;
    internal ScheduleStatus status;
    internal List<Court> listOfCourts;
    internal List<Court> listOfAvailableCourts;
    internal List<Booking> listOfBookings;

    private DailySchedule()
    {
        scheduleId = Guid.NewGuid();
        scheduleDate = DateTime.Today;
        availableFrom = new TimeSpan(15, 0, 0);
        availableUntil = new TimeSpan(22, 0, 0);
        scheduleDate = DateTime.Today;
        status = ScheduleStatus.Draft;
        listOfCourts = [];
        listOfAvailableCourts = [];
        listOfBookings = [];

    }
    
    

    public static Result<DailySchedule> CreateSchedule()
    {
        var dailySchedule = new DailySchedule();
        return Result<DailySchedule>.Ok(dailySchedule);
    }   
    
    private static Result<DailySchedule> FindSchedule(Guid scheduleId, List<DailySchedule> schedules)
    {
        var schedule = schedules.FirstOrDefault(s => s.scheduleId == scheduleId);
        return schedule == null
            ? Result<DailySchedule>.Fail(ErrorMessage.ScheduleNotFound()._message)
            : Result<DailySchedule>.Ok(schedule);
    }
    
    private Result ValidateScheduleForCourtAddition()
    {
        if (scheduleDate < DateTime.Today)
        {
            return Result.Fail(ErrorMessage.PastScheduleCannotBeUpdated()._message);
        }

        if (status != ScheduleStatus.Draft && status != ScheduleStatus.Active)
        {
            return Result.Fail(ErrorMessage.InvalidScheduleStatus()._message);
        }

        return Result.Ok();
    }
    
    public Result AddAvailableCourt(Guid scheduleId, string courtName, List<DailySchedule> schedules)
    {
        var scheduleResult = FindSchedule(scheduleId, schedules);
        if (!scheduleResult.Success)
        {
            return Result.Fail(scheduleResult.ErrorMessage); 
        }

        var schedule = scheduleResult.Data;

        var validationResult = schedule.ValidateScheduleForCourtAddition();
        if (!validationResult.Success)
        {
            return validationResult;
        }

        var courtNameResult = CourtName.Create(courtName);
        if (!courtNameResult.Success)
        {
            return Result.Fail(courtNameResult.ErrorMessage);
        }

        var courtCheckResult = schedule.HasCourt(courtNameResult.Data);
        if (!courtCheckResult.Success)
        {
            return courtCheckResult;
        }

        schedule.listOfCourts.Add(Court.Create(courtNameResult.Data));
        return Result.Ok();
    }
    
    private Result HasCourt(CourtName name)
    {
        if (listOfCourts.Any(court => court.Name.Value == name.Value))
        {
            return Result.Fail(ErrorMessage.CourtAlreadyExists()._message);
        }
        return Result.Ok();
    }
}
