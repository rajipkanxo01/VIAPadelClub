﻿using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Common.BaseClasses;
using VIAPadelClub.Core.Tools.OperationResult;
// ReSharper disable ParameterHidesMember

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

public class DailySchedule : AggregateRoot
{
    internal Guid scheduleId;
    internal DateOnly scheduleDate;
    internal TimeOnly availableFrom;
    internal TimeOnly availableUntil;
    internal List<(TimeOnly start, TimeOnly end)> vipTimeRanges = new();
    internal ScheduleStatus status;
    internal List<Court> listOfCourts;
    internal List<Court> listOfAvailableCourts;
    internal List<Booking> listOfBookings;
    internal bool isDeleted;

    private DailySchedule()
    {
        scheduleId = Guid.NewGuid();
        availableFrom = new TimeOnly(15, 0, 0);
        availableUntil = new TimeOnly(22, 0, 0);
        status = ScheduleStatus.Draft;
        listOfCourts = [];
        listOfAvailableCourts = [];
        listOfBookings = [];
        isDeleted = false;
    }

    public Guid Id => scheduleId;
    public List<Booking> listOfbookings => listOfBookings;
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

        schedule.listOfAvailableCourts.Add(Court.Create(courtNameResult.Data));
        scheduleFinder.AddSchedule(schedule);
        return Result.Ok();
    }

    private Result HasCourt(CourtName name)
    {
        if (listOfAvailableCourts.Any(court => court.Name.Value == name.Value))
        {
            return Result.Fail(ErrorMessage.CourtAlreadyExists()._message);
        }

        return Result.Ok();
    }
    
    public Result RemoveAvailableCourt(Court court, IDateProvider dateProvider, IScheduleFinder scheduleFinder)
    {
        var scheduleResult = scheduleFinder.FindSchedule(scheduleId);
        if (!scheduleResult.Success)
        {
            return Result.Fail(scheduleResult.ErrorMessage);
        }

        var schedule = scheduleResult.Data;
        
        if (schedule.scheduleDate < dateProvider.Today() && (schedule.status == ScheduleStatus.Draft || schedule.status == ScheduleStatus.Active))
        {
            return Result.Fail(ErrorMessage.PastScheduleCannotBeUpdated()._message);
        }
        
        if (!schedule.listOfAvailableCourts.Contains(court))
        {
            return Result.Fail(ErrorMessage.NoCourtAvailable()._message);
        }
        if (schedule.scheduleId != scheduleId)
        {
            return Result.Fail(ErrorMessage.ScheduleNotFound()._message);
        }
        // Fetch all bookings for the given court on the schedule date
        var bookingsResult = schedule.listOfBookings.Where(booking => booking.Court.Name.Equals(court.Name));
    
        var bookings = bookingsResult.ToList();
        var currentTime = TimeOnly.FromDateTime(DateTime.Now);

        
        // F3 – Booking is ongoing
        var bookingsList = bookings.ToList();
        if (bookingsList.Any(booking => booking.StartTime < currentTime && booking.EndTime > currentTime))
        {
            return Result.Fail(ErrorMessage.ActiveCourtCannotBeRemoved()._message);
        }
        
        // F5 – Bookings later on the same day
        if (bookingsList.Any(booking => booking.StartTime >= currentTime))
        {
            return Result.Fail(ErrorMessage.CourtWithLaterBookingsCannotBeRemoved()._message);
        }

        if (bookingsList.All(booking => booking.EndTime <= currentTime))
        {
            schedule.listOfAvailableCourts.Remove(court);
            return Result.Ok();
        }
        schedule.listOfAvailableCourts.Remove(court);
        return Result.Ok();
    }

    private Result ValidateScheduleForCourtAddition(DateOnly today)
    {
        if (scheduleDate < today)
        {
            return Result.Fail(ErrorMessage.PastScheduleCannotBeUpdated()._message);
        }

        if (status != ScheduleStatus.Draft && status != ScheduleStatus.Active)
        {
            return Result.Fail(ErrorMessage.InvalidScheduleStatus()._message);
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
            return Result.Fail(ErrorMessage.InvalidTimeSlot()._message);
        }

        //  Chosen time span is outside of daily schedule time span 
        if (vipStartTime < availableFrom || vipEndTime < availableFrom || vipStartTime > availableUntil ||
            vipEndTime > availableUntil)
        {
            return Result.Fail(ErrorMessage.VipTimeOutsideSchedule()._message);
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
            return Result.Fail(ErrorMessage.PastScheduleCannotBeActivated()._message);
        
        if(listOfCourts.Count==0)
            return Result.Fail(ErrorMessage.NoCourtAvailable()._message);

        if (status == ScheduleStatus.Active)
            return Result.Fail(ErrorMessage.ScheduleAlreadyActive()._message);
        
        if (isDeleted)
            return Result.Fail(ErrorMessage.ScheduleIsDeleted()._message);
        
        status = ScheduleStatus.Active;
        return Result.Ok();
    }
    public Result DeleteSchedule(IDateProvider dateProvider)
    {
        if (isDeleted)
            return Result.Fail(ErrorMessage.ScheduleAlreadyDeleted()._message);

        if (scheduleDate < dateProvider.Today())
            return Result.Fail(ErrorMessage.PastScheduleCannotBeDeleted()._message);
        
        if (scheduleDate == dateProvider.Today() && status == ScheduleStatus.Active)
            return Result.Fail(ErrorMessage.SameDayActiveScheduleCannotBeDeleted()._message);

        isDeleted = true;

        foreach (var booking in listOfBookings)
        {
            booking.CancelBooking();
            Console.WriteLine($"**NOTIFICATION** Your booking on {scheduleDate} has been canceled due to schedule deletion.");
        }
            
        listOfCourts.Clear();
        
        return Result.Ok();
    }
    
    public Result UpdateScheduleDateAndTime(DateOnly date, TimeOnly startTime, TimeOnly endTime, IDateProvider dateProvider)
    {
        if (date <= dateProvider.Today())
            return Result.Fail(ErrorMessage.ScheduleCannotBeUpdatedWithPastDate()._message);

        if (endTime <= startTime)
            return Result.Fail(ErrorMessage.ScheduleEndDateMustBeAfterStartDate()._message);

        if ((endTime - startTime) < TimeSpan.FromMinutes(60))
            return Result.Fail(ErrorMessage.ScheduleInvalidTimeSpan()._message);
        
        if (status == ScheduleStatus.Active)
            return Result.Fail(ErrorMessage.InvalidScheduleUpdateStatus()._message);

        if ((startTime.Minute != 0 && startTime.Minute != 30) || (endTime.Minute != 0 && endTime.Minute != 30))
            return Result.Fail(ErrorMessage.InvalidScheduleTimeSpan()._message);

        scheduleDate = date;
        availableFrom = startTime;
        availableUntil = endTime;

        return Result.Ok();
    }
}