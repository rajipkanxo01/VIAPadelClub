using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Common.BaseClasses;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;

using Players.Values;

public class Booking : Entity
{
    internal Guid BookingId { get; }
    internal Email BookedBy { get; }
    internal Court Court { get; }
    internal TimeOnly StartTime { get; }
    internal TimeOnly EndTime { get; }
    internal DateOnly BookedDate { get; }
    
    protected Booking(Guid bookingId, Player player, Court court, TimeOnly startTime, TimeOnly endTime, DateOnly bookedDate) : base(bookingId)
    {
        BookedBy = player.email;
        Court = court;
        StartTime = startTime;
        EndTime = endTime;
        BookedDate = bookedDate;
    }

    public void CancelBooking()
    {
        throw new NotImplementedException();
    }
}