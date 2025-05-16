using VIAPadelClub.Core.QueryContracts.Contract;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.QueryContracts.Queries;

public class PlayerPageOverview
{
    public record Query(string PlayerEmail) : IQuery<Result<Answer>>;

    public record Answer(
        string FirstName,
        string LastName,
        string Email,
        string? VipEndDate,
        string ProfilePictureUrl,
        int FutureBookingCount,
        List<BookingSummary> UpcomingBookings,
        List<BookingSummary> PastBookings
    );

    public record BookingSummary(
        string Date,
        string StartTime,
        string EndTime
    );
    
}