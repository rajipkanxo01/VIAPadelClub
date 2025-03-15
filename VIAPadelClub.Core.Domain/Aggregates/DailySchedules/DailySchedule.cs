﻿using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Common.BaseClasses;
using VIAPadelClub.Core.Tools.OperationResult;

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

public class DailySchedule : AggregateRoot
{
    public Guid scheduleId;
    internal DateTime scheduleDate;
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
        scheduleDate = DateTime.Today;
        availableFrom = new TimeOnly(15, 0, 0);
        availableUntil = new TimeOnly(22, 0, 0);
        status = ScheduleStatus.Draft;
        listOfCourts = [];
        listOfAvailableCourts = [];
        listOfBookings = [];
        isDeleted = false;
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

    public Result DeleteSchedule()
    {
        if (isDeleted)
            return Result.Fail(ErrorMessage.ScheduleAlreadyDeleted()._message);

        if (scheduleDate < DateTime.Today)
            return Result.Fail(ErrorMessage.PastScheduleCannotBeDeleted()._message);
        
        if (scheduleDate == DateTime.Today && status == ScheduleStatus.Active)
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
}