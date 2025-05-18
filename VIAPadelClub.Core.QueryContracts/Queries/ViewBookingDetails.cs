using VIAPadelClub.Core.QueryContracts.Contract;

namespace VIAPadelClub.Core.QueryContracts.Queries;

public static class ViewBookingDetails
{
    public record Query(Guid BookingId) : IQuery<Answer>;

    public record Answer(
        Guid BookingId,
        string BookedBy,
        string CourtName,
        Guid ScheduleId,
        string Duration,
        string StartTime,
        string EndTime,
        string BookedDate,
        string BookingStatus
    );
}