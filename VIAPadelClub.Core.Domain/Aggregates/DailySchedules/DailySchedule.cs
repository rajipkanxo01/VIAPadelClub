using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Domain.Common.BaseClasses;
using VIAPadelClub.Core.Tools.OperationResult;
// ReSharper disable ParameterHidesMember

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

using Players;

public class DailySchedule : AggregateRoot
{
    internal Guid scheduleId;
    internal DateOnly scheduleDate;
    internal TimeOnly availableFrom;
    internal TimeOnly availableUntil;
    internal ScheduleStatus status;
    internal bool isDeleted;

    internal List<Court> listOfCourts;
    internal List<Court> listOfAvailableCourts;
    internal List<Booking> listOfBookings;
    internal List<(TimeOnly start, TimeOnly end)> vipTimeRanges = new();

    private DailySchedule()
    {
        scheduleId = Guid.NewGuid();
        availableFrom = new TimeOnly(15, 0, 0);
        availableUntil = new TimeOnly(22, 0, 0);
        status = ScheduleStatus.Draft;
        listOfCourts = new List<Court>();
        listOfAvailableCourts = [];
        listOfBookings = new List<Booking>();
        isDeleted = false;
    }

    public Guid Id => scheduleId;
    public static Result<DailySchedule> CreateSchedule(IDateProvider dateProvider)
    {
        var today = dateProvider.Today();
        
        var dailySchedule = new DailySchedule()
        {
            scheduleDate = today
        };
        
        return Result<DailySchedule>.Ok(dailySchedule);
    }

    public Result AddAvailableCourt(Court court, IDateProvider dateProvider, IScheduleFinder scheduleFinder)
    {
        var scheduleResult = scheduleFinder.FindSchedule(scheduleId);
        if (!scheduleResult.Success)
        {
            return Result.Fail(scheduleResult.ErrorMessage);
        }

        var schedule = scheduleResult.Data;

        var validationResult = schedule.ValidateScheduleForCourtAddition(dateProvider.Today());
        if (!validationResult.Success)
        {
            return validationResult;
        }

        var courtNameResult = CourtName.Create(court.Name.Value);
        if (!courtNameResult.Success)
        {
            return Result.Fail(courtNameResult.ErrorMessage);
        }

        var courtCheckResult = schedule.HasCourt(courtNameResult.Data);
        if (!courtCheckResult.Success)
        {
            return courtCheckResult;
        }

        schedule.listOfAvailableCourts.Add(Court.Create(courtNameResult.Data).Data);
        scheduleFinder.AddSchedule(schedule);
        return Result.Ok();
    }

    private Result HasCourt(CourtName name)
    {
        if (listOfAvailableCourts.Any(court => court.Name.Value == name.Value))
        {
            return Result.Fail(DailyScheduleError.CourtAlreadyExists()._message);
        }

        return Result.Ok();
    }
    
    public Result RemoveAvailableCourt(Court court, IDateProvider dateProvider, TimeOnly timeOfRemoval)
    {
        // Ensure the schedule is for today or the future
        if (scheduleDate < dateProvider.Today() && (status == ScheduleStatus.Draft || status == ScheduleStatus.Active))
        {
            return Result.Fail(DailyScheduleError.PastScheduleCannotBeUpdated()._message);
        }

        // Ensure the court exists in the schedule
        if (listOfAvailableCourts.All(c => c.Name.Value != court.Name.Value))
        {
            return Result.Fail(DailyScheduleError.NoCourtAvailable()._message);
        }

        // Fetch all bookings for the given court on the schedule date
        var bookings = listOfBookings
            .Where(booking => booking.Court.Name.Equals(court.Name))
            .ToList();

        // F3 – Player is ongoing (court is currently in use)
        if (bookings.Any(booking => booking.StartTime < timeOfRemoval && booking.EndTime > timeOfRemoval))
        {
            return Result.Fail(DailyScheduleError.ActiveCourtCannotBeRemoved()._message);
        }

        // F5 – Check if there are bookings later on the same day
        if (bookings.Any(booking => booking.StartTime <= timeOfRemoval))
        {
            return Result.Fail(DailyScheduleError.CourtWithLaterBookingsCannotBeRemoved()._message);
        }

        // If no future bookings exist, remove the court
        listOfAvailableCourts.Remove(court);
        listOfCourts.Remove(court);
        return Result.Ok();
    }


    private Result ValidateScheduleForCourtAddition(DateOnly today)
    {
        if (scheduleDate < today)
        {
            return Result.Fail(DailyScheduleError.PastScheduleCannotBeUpdated()._message);
        }

        if (status != ScheduleStatus.Draft && status != ScheduleStatus.Active)
        {
            return Result.Fail(DailyScheduleError.InvalidScheduleStatus()._message);
        }

        return Result.Ok();
    }

    public Result UpdateSchedule(DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        availableFrom = startTime;
        availableUntil = endTime;

        return Result.Ok();
    }

    public Result AddVipTimeSlots(TimeOnly vipStartTime, TimeOnly vipEndTime)
    {
        //  Incorrect format of .00 or .30 
        if ((vipStartTime.Minute != 30 && vipStartTime.Minute != 0) ||
            (vipEndTime.Minute != 30 && vipEndTime.Minute != 0))
        {
            return Result.Fail(DailyScheduleError.InvalidTimeSlot()._message);
        }

        //  Chosen time span is outside of daily schedule time span 
        if (vipStartTime < availableFrom || vipEndTime < availableFrom || vipStartTime > availableUntil ||
            vipEndTime > availableUntil)
        {
            return Result.Fail(DailyScheduleError.VipTimeOutsideSchedule()._message);
        }

        var timeRanges = vipTimeRanges.Where(vipRange =>
            vipRange.end == vipStartTime || vipRange.start == vipEndTime || // checks for border existing
            (vipRange.start <= vipStartTime && vipRange.end >= vipStartTime) || // new vip start inside existing
            (vipRange.start <= vipEndTime && vipRange.end >= vipEndTime) || // new vip end inside existing
            (vipStartTime <= vipRange.start && vipEndTime >= vipRange.end) // checks if both start and end time falls inside existing range

        ).ToList();

        if (timeRanges.Any())
        {
            var newStartTime = timeRanges.Min(vip => vip.start < vipStartTime ? vip.start : vipStartTime);
            var newEndTime = timeRanges.Max(vip => vip.end > vipEndTime ? vip.end : vipEndTime);

            vipTimeRanges.RemoveAll(vip => timeRanges.Contains(vip));
            vipTimeRanges.Add((newStartTime, newEndTime));
        }
        else
        {
            vipTimeRanges.Add((vipStartTime, vipEndTime));
        }

        return Result.Ok();
    }

    public Result Activate(IDateProvider dateProvider)
    {
        if(scheduleDate < dateProvider.Today())
            return Result.Fail(DailyScheduleError.PastScheduleCannotBeActivated()._message);
        
        if(listOfCourts.Count==0)
            return Result.Fail(DailyScheduleError.NoCourtAvailable()._message);

        if (status == ScheduleStatus.Active)
            return Result.Fail(DailyScheduleError.ScheduleAlreadyActive()._message);
        
        if (isDeleted)
            return Result.Fail(DailyScheduleError.ScheduleIsDeleted()._message);

        listOfAvailableCourts = listOfCourts;
        status = ScheduleStatus.Active;
        return Result.Ok();
    }
    public Result DeleteSchedule(IDateProvider dateProvider, ITimeProvider timeProvider)
    {
        if (isDeleted)
            return Result.Fail(DailyScheduleError.ScheduleAlreadyDeleted()._message);

        if (scheduleDate < dateProvider.Today())
            return Result.Fail(DailyScheduleError.PastScheduleCannotBeDeleted()._message);
        
        if (scheduleDate == dateProvider.Today() && status == ScheduleStatus.Active)
            return Result.Fail(DailyScheduleError.SameDayActiveScheduleCannotBeDeleted()._message);

        isDeleted = true;

        foreach (var booking in listOfBookings)
        {
            booking.Cancel(dateProvider, timeProvider, booking.BookedBy);
            Console.WriteLine($"**NOTIFICATION** Your booking on {scheduleDate} has been canceled due to schedule deletion.");
        }
            
        listOfCourts.Clear();
        
        return Result.Ok();
    }
    
    public Result UpdateScheduleDateAndTime(DateOnly date, TimeOnly startTime, TimeOnly endTime, IDateProvider dateProvider)
    {
        if (date <= dateProvider.Today())
            return Result.Fail(DailyScheduleError.ScheduleCannotBeUpdatedWithPastDate()._message);

        if (endTime <= startTime)
            return Result.Fail(DailyScheduleError.ScheduleEndDateMustBeAfterStartDate()._message);

        if ((endTime - startTime) < TimeSpan.FromMinutes(60))
            return Result.Fail(DailyScheduleError.ScheduleInvalidTimeSpan()._message);
        
        if (status == ScheduleStatus.Active)
            return Result.Fail(DailyScheduleError.InvalidScheduleUpdateStatus()._message);

        if ((startTime.Minute != 0 && startTime.Minute != 30) || (endTime.Minute != 0 && endTime.Minute != 30))
            return Result.Fail(DailyScheduleError.ScheduleInvalidTimeSpan()._message);

        scheduleDate = date;
        availableFrom = startTime;
        availableUntil = endTime;

        return Result.Ok();
    }

  public Result<Booking> BookCourt (Email bookedByPlayer, Court court, TimeOnly startTime, TimeOnly endTime, IDateProvider dateProvider, IPlayerFinder playerFinder, IScheduleFinder scheduleFinder)
    {
        var booking = Booking.Create(scheduleId, court, startTime, endTime,bookedByPlayer, scheduleFinder, playerFinder);
        if (!booking.Success)
        {
            return Result<Booking>.Fail(booking.ErrorMessage);
        }
        return Result<Booking>.Ok(booking.Data);
    }

  public Result CancelBooking(Guid bookingId, IDateProvider dateProvider, ITimeProvider timeProvider, Email playerMakingCancel)
    {
        var booking = listOfBookings.FirstOrDefault(booking => booking.BookingId == bookingId);

        if (booking == null)
        {
            return Result.Fail(DailyScheduleError.BookingNotFound()._message);
        }

        return booking.Cancel(dateProvider,timeProvider, playerMakingCancel);
    }
}