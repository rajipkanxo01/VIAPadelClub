using VIAPadelClub.Core.QueryContracts.Contract;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.QueryContracts.Queries;

public static class ViewBookingDetails
{
    public record Query(Guid BookingId) : IQuery<Result<Answer>>;

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