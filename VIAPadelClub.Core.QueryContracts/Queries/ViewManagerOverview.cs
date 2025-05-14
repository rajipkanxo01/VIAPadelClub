using VIAPadelClub.Core.QueryContracts.Contract;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.QueryContracts.Queries;

public class ViewManagerOverview
{
    public record Query(string MonthName) : IQuery<Result<Answer>>;
    public record Answer(List<ScheduleView> Schedules);

    public record ScheduleView(
        string Id,
        string Date,
        string Status,
        int CourtCount
    );
}