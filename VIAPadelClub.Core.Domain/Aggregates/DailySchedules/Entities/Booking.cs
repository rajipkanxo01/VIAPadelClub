using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Common.BaseClasses;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;

public class Booking : Entity
{
    internal Guid BookingId { get; }
    internal Player BookedBy { get; }
    protected Booking(Guid id, Player bookedBy) : base(id)
    {
        BookingId = id;
        BookedBy = bookedBy;
    }

    public void CancelBooking()
    {
        throw new NotImplementedException();
    }
}