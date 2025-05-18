using VIAPadelClub.Core.QueryContracts.Contract;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.QueryContracts.Queries;

public class ViewSchedulePlayerOverview
{
    public record Query(string Date) : IQuery<Result<Answer>>;
    public record Answer(
        string ScheduleId,
        string Date,
        string Status,
        List<CourtView> AvailableCourts,
        List<VipTimeRangeView> VipTimeRanges,
        List<BookingView> Bookings
    );
    
    public record CourtView(
        string Name
    );

    public record VipTimeRangeView(
        string Start,
        string End
    );

    public record BookingView(
        string BookingId,
        string PlayerEmail,
        string CourtName,
        string StartTime,
        string EndTime,
        string Status
    );
}