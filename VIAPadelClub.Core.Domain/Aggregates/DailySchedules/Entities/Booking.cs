using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Domain.Common.BaseClasses;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;

public class Booking : Entity
{
    internal Guid BookingId { get; }
    internal Email BookedBy { get; }
    internal TimeOnly BookingStartTime { get; }
    internal TimeOnly BookingEndTime { get; }
    internal DateOnly BookingDate { get; }
    internal string BookedCourtName { get; }
    internal BookingStatus BookingStatus { get; set; }

    protected Booking(Guid id , string bookedCourtName, Email bookedBy, TimeOnly bookingStartTime, TimeOnly bookingEndTime, DateOnly bookingDate) : base(id)
    {
        BookingId = id;
        BookedBy = bookedBy;
        BookingStartTime = bookingStartTime;
        BookingEndTime = bookingEndTime;
        BookingDate = bookingDate;
        BookedCourtName = bookedCourtName;
    }

    public Result Cancel(IDateProvider dateProvider, ITimeProvider timeProvider, Email playerMakingCancel)
    {
        var currentDate = dateProvider.Today();
        var currentTime = timeProvider.CurrentTime();

        // Check if the booking is already in the past
        if (currentDate > BookingDate || (currentDate == BookingDate && currentTime >= BookingStartTime))
        {
            return Result.Fail(ErrorMessage.CannotCancelPastBooking()._message);
        }

        // Check if cancellation is too late (less than 1 hour before booking starts)
        if (currentDate == BookingDate && (BookingStartTime.ToTimeSpan() - currentTime.ToTimeSpan()).TotalHours < 1)
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


    public static Result<Booking> Create(Guid id, string courtName, TimeOnly bookingStartTime, TimeOnly bookingEndTime, DateOnly bookingDate, Email playerEmail, IScheduleFinder scheduleFinder)
    {
        var booking = new Booking(Guid.NewGuid(), courtName,playerEmail, bookingStartTime, bookingEndTime, bookingDate);
        return Result<Booking>.Ok(booking);
    }
}