using System.Runtime.CompilerServices;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Domain.Common.BaseClasses;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;

using Players.Values;

public class Booking : Entity
{
    internal Guid BookingId { get; }
    internal Email BookedBy { get; }
    internal Court Court { get; }
    internal int Duration{ get; }
    internal TimeOnly StartTime { get; }    
    internal TimeOnly EndTime { get; }
    internal DateOnly BookedDate { get; }
    internal BookingStatus BookingStatus { get; private set; }


    private Booking(Guid id,Email bookedBy, Court court, int duration, DateOnly bookedDate, TimeOnly startTime, TimeOnly endTime) : base(id)
    {
        BookingId = id;
        BookedBy = bookedBy;
        Court = court;
        Duration = duration;
        BookedDate = bookedDate;
        StartTime = startTime;
        EndTime = endTime;
        BookingStatus = BookingStatus.Active;
    }

    public static Result<Booking> Create(Guid scheduleId,Court court,TimeOnly startTime, TimeOnly endTime,Email email,IScheduleFinder scheduleFinder,IPlayerFinder playerFinder)
    {
        var scheduleResult = scheduleFinder.FindSchedule(scheduleId);
        if (!scheduleResult.Success)
        {
            return Result<Booking>.Fail(scheduleResult.ErrorMessage);
        }
        
        var schedule = scheduleResult.Data;
        
        if (schedule.isDeleted || schedule.status != ScheduleStatus.Active) //F1 and 2
            return Result<Booking>.Fail(ErrorMessage.ScheduleNotActive()._message);

        if (!schedule.listOfAvailableCourts.Contains(court)) //F4
            return Result<Booking>.Fail(ErrorMessage.CourtDoesntExistInSchedule()._message); 
        
        //F5- Booking start time before schedule start time 
        if (startTime < schedule.availableFrom)
        {
            return Result<Booking>.Fail(ErrorMessage.BookingStartTimeBeforeScheduleStartTime()._message);
        }
        
        //F6- Booking end time after schedule start time
        if (endTime < schedule.availableFrom)
        {
            return Result<Booking>.Fail(ErrorMessage.BookingEndTimeAfterScheduleStartTime()._message);
        }
        
        //F7- Booking start time after schedule's end time
        if (startTime > schedule.availableUntil)
        {
            return Result<Booking>.Fail(ErrorMessage.BookingStartTimeAfterScheduleStartTime()._message);
        }
        
        //F8- Booking end time after schedule's end time
        if (endTime > schedule.availableUntil)
        {
            return Result<Booking>.Fail(ErrorMessage.BookingEndTimeAfterScheduleEndTime()._message);
        }
        
        if ((startTime.Minute != 0 && startTime.Minute != 30) || (endTime.Minute != 0 && endTime.Minute != 30))  //F9
            return Result<Booking>.Fail(ErrorMessage.InvalidBookingTimeSpan()._message);
        
        var player = playerFinder.FindPlayer(email.Value);
        
        if (!player.Success)
        {
            return Result<Booking>.Fail(player.ErrorMessage);
        }
        
        var duration = endTime - startTime; //F10 and F12
        if (duration < TimeSpan.FromHours(1) || duration > TimeSpan.FromHours(3)) 
            return Result<Booking>.Fail(ErrorMessage.BookingDurationError()._message);
        
        if (schedule.listOfBookings.Any(b =>  //F11
                b.Court == court &&
                !(endTime <= b.StartTime || startTime >= b.EndTime)))
        {
            return Result<Booking>.Fail(ErrorMessage.BookingCannotBeOverlapped()._message);
        }
        
        var playerResult = player.Data;
        //F13- player is quarantined and the selected date of booking is before the quarantine is ended
        if (playerResult.isQuarantined && playerResult.activeQuarantine?.EndDate >= schedule.scheduleDate)
        {
            return Result<Booking>.Fail(ErrorMessage.QuarantinePlayerCannotBookCourt()._message);
        }
        
        //F14- player is blacklisted
        if (playerResult.isBlackListed)
        {
            return Result<Booking>.Fail(ErrorMessage.PlayerIsBlacklisted()._message);
        }
        
        //F15- if non VIP player tries to book court in VIP time then should be rejected
        foreach (var vipTime in schedule.vipTimeRanges)
        {
            if (startTime<vipTime.end && endTime>vipTime.start)
            {
                if (playerResult.vipMemberShip is null)
                {
                    return Result<Booking>.Fail(ErrorMessage.NonVipMemberCannotBookInVipTimeSlot()._message);
                }
            }
        }
        
        if (schedule.listOfBookings.Count(b => b.BookedBy.Value == player.Data.email.Value && b.BookedDate == schedule.scheduleDate) >= 1) //F17
            return Result<Booking>.Fail(ErrorMessage.BookingLimitExceeded()._message);

        //F18- Booking leave hole less than one hour

        var existingBookings = schedule.listOfBookings.Where(booking => booking.Court.Name.Equals(court.Name));

        foreach (var booking in existingBookings)
        {
            //Ensure at least 1-hour gap before the new booking
            if (booking.EndTime <= startTime && (startTime - booking.EndTime).TotalMinutes < 60)
            {
                return Result<Booking>.Fail(ErrorMessage.OneHourGapShouldBeBeforeNewBooking()._message);
            }

            //Ensure at least 1-hour gap after the new booking
            if (booking.StartTime >= endTime && (booking.StartTime - endTime).TotalMinutes < 60)
            {
                return Result<Booking>.Fail(ErrorMessage.OneHourGapShouldBeAfterAnotherBooking()._message);
            }

            //Check for any overlap
            if (startTime < booking.EndTime || endTime > booking.StartTime)
            {
                return Result<Booking>.Fail(ErrorMessage.BookingCannotBeOverlapped()._message);
            }
        }
        
        // Get daily schedule available time
        var scheduleStart = schedule.availableFrom;
        var scheduleEnd = schedule.availableUntil;
        
        // Check if booking creates small gaps with the daily schedule time
        if ((startTime - scheduleStart).TotalMinutes < 60 && startTime > scheduleStart)
        {
            return Result<Booking>.Fail(ErrorMessage.OneHourGapBetweenScheduleStartTimeAndBookingStartTime()._message);
        }

        if ((scheduleEnd - endTime).TotalMinutes < 60 && endTime < scheduleEnd)
        {
            return Result<Booking>.Fail(ErrorMessage.OneHourGapBetweenScheduleEndTimeAndBookingEndTime()._message);
        }
        var newBooking = new Booking(Guid.NewGuid(), email, court, (int)(endTime - startTime).TotalMinutes, schedule.scheduleDate, startTime, endTime);
        
        return Result<Booking>.Ok(newBooking);
    }
    
    public Result Cancel(IDateProvider dateProvider, ITimeProvider timeProvider, Email playerMakingCancel)
    {
        var currentDate = dateProvider.Today();
        var currentTime = timeProvider.CurrentTime();

        // Check if the booking is already in the past
        if (currentDate > BookedDate || (currentDate == BookedDate && currentTime >= StartTime))
        {
            return Result.Fail(ErrorMessage.CannotCancelPastBooking()._message);
        }

        // Check if cancellation is too late (less than 1 hour before booking starts)
        if (currentDate == BookedDate && (StartTime.ToTimeSpan() - currentTime.ToTimeSpan()).TotalHours < 1)
        {
            return Result.Fail(ErrorMessage.CancellationTooLate()._message);
        }

        // Check if player owns the booking
        if (playerMakingCancel != BookedBy)
        {
            return Result.Fail(ErrorMessage.BookingOwnershipViolation()._message);
        }

        BookingStatus = BookingStatus.Cancelled;
        return Result.Ok();
    }
}