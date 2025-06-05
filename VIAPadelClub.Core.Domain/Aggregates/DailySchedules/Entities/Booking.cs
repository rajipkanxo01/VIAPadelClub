using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Common.BaseClasses;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;

using Players.Values;

public class Booking : Entity
{
    public BookingId BookingId { get; private set; }
    // internal Guid Id { get; }

    internal Email BookedBy { get; }
    internal Court Court { get; }
    internal int Duration { get; }
    internal TimeOnly StartTime { get; }
    internal TimeOnly EndTime { get; }
    internal DateOnly BookedDate { get; }
    internal BookingStatus BookingStatus { get; private set; }

    private Booking()
    {
        // EFC only.
    }

    private Booking(BookingId id, Email bookedBy, Court court, int duration, DateOnly bookedDate, TimeOnly startTime,
        TimeOnly endTime)
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

    public static async Task<Result<Booking>> Create(
        ScheduleId scheduleId,
        Court court,
        TimeOnly startTime,
        TimeOnly endTime,
        Email email,
        IScheduleFinder scheduleFinder,
        IPlayerFinder playerFinder)
    {
        var scheduleResult = await scheduleFinder.FindSchedule(scheduleId);
        if (!scheduleResult.Success)
            return Result<Booking>.Fail(scheduleResult.ErrorMessage);

        var schedule = scheduleResult.Data;
        if (!IsValidSchedule(schedule))
            return Result<Booking>.Fail(DailyScheduleError.ScheduleNotActive()._message);

        if (!CourtExists(schedule, court))
            return Result<Booking>.Fail(DailyScheduleError.CourtDoesntExistInSchedule()._message);

        if (!IsValidTimeRange(schedule, startTime, endTime))
            return Result<Booking>.Fail(DailyScheduleError.BookingEndTimeAfterScheduleEndTime()._message);

        var playerResult = playerFinder.FindPlayer(email);
        if (!playerResult.Success)
            return Result<Booking>.Fail(playerResult.ErrorMessage);

        var player = playerResult.Data;
        if (!IsBookingAllowed(schedule, player, court, startTime, endTime))
            return Result<Booking>.Fail(DailyScheduleError.BookingCannotBeOverlapped()
                ._message);

        var booking = new Booking(
            BookingId.Create(),
            email,
            court,
            (int)(endTime - startTime).TotalMinutes,
            schedule.scheduleDate,
            startTime,
            endTime);

        return Result<Booking>.Ok(booking);
    }

    private static bool IsValidSchedule(DailySchedule schedule) =>
        !schedule.isDeleted && schedule.status == ScheduleStatus.Active;

    private static bool CourtExists(DailySchedule schedule, Court court) =>
        schedule.listOfCourts.Any(c =>
            string.Equals(c.Name.Value.Trim(), court.Name.Value.Trim(), StringComparison.OrdinalIgnoreCase));

    private static bool IsValidTimeRange(DailySchedule schedule, TimeOnly start, TimeOnly end)
    {
        if (start < schedule.availableFrom)
            return false;

        if (end < schedule.availableFrom || end > schedule.availableUntil)
            return false;

        if (start > schedule.availableUntil)
            return false;

        if ((start.Minute != 0 && start.Minute != 30) || (end.Minute != 0 && end.Minute != 30))
            return false;

        var duration = end - start;
        if (duration < TimeSpan.FromHours(1) || duration > TimeSpan.FromHours(3))
            return false;

        if ((start - schedule.availableFrom).TotalMinutes < 60 && start > schedule.availableFrom)
            return false;

        if ((schedule.availableUntil - end).TotalMinutes < 60 && end < schedule.availableUntil)
            return false;

        return true;
    }

    private static bool IsBookingAllowed(DailySchedule schedule, Player player, Court court, TimeOnly start,
        TimeOnly end)
    {
        if (player.isQuarantined && player.activeQuarantine?.EndDate >= schedule.scheduleDate)
            return false;

        if (player.isBlackListed)
            return false;

        if (schedule.vipTimeRanges.Any(vip => start < vip.End && end > vip.Start) &&
            player.vipMemberShip == null)
            return false;

        if (schedule.listOfBookings.Count(b =>
                b.BookedBy.Value == player.email.Value && b.BookedDate == schedule.scheduleDate) >= 1)
            return false;

        var bookings = schedule.listOfBookings.Where(b => b.Court.Name.Equals(court.Name));

        foreach (var b in bookings)
        {
            if ((b.EndTime <= start && (start - b.EndTime).TotalMinutes < 60) ||
                (b.StartTime >= end && (b.StartTime - end).TotalMinutes < 60) ||
                (start < b.EndTime && end > b.StartTime))
                return false;
        }

        return true;
    }


    public Result Cancel(IDateProvider dateProvider, ITimeProvider timeProvider, Email playerMakingCancel)
    {
        var currentDate = dateProvider.Today();
        var currentTime = timeProvider.CurrentTime();

        // Check if the booking is already in the past
        if (currentDate > BookedDate || (currentDate == BookedDate && currentTime >= StartTime))
        {
            return Result.Fail(DailyScheduleError.CannotCancelPastBooking()._message);
        }

        // Check if cancellation is too late (less than 1 hour before booking starts)
        if (currentDate == BookedDate && (StartTime.ToTimeSpan() - currentTime.ToTimeSpan()).TotalHours < 1)
        {
            return Result.Fail(DailyScheduleError.CancellationTooLate()._message);
        }

        // Check if player owns the booking
        if (!playerMakingCancel.Equals(BookedBy))
        {
            return Result.Fail(DailyScheduleError.BookingOwnershipViolation()._message);
        }

        BookingStatus = BookingStatus.Cancelled;
        return Result.Ok();
    }

    internal void CancelDueToQuarantine()
    {
        if (BookingStatus == BookingStatus.Active)
        {
            BookingStatus = BookingStatus.Cancelled;
            Console.WriteLine(
                $"**NOTIFICATION** Booking on {BookedDate} at {StartTime} was cancelled due to quarantine.");
        }
    }
}