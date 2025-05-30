using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.ObjectMapper;
using VIAPadelClub.Presentation.WebApi.Endpoints.Queries;

namespace VIAPadelClub.Presentation.WebApi.ObjectMapping;

public class ViewManagerOverviewAnswerToResponseMapping 
    : IMappingConfig<ViewManagerOverview.Answer, ViewManagerOverviewResponse>
{
    public ViewManagerOverviewResponse Map(ViewManagerOverview.Answer input)
    {
        var schedules = input.Schedules.Select(schedule =>
            new Schedule(
                schedule.Id,
                schedule.Date,
                schedule.Status,
                schedule.CourtCount
            )).ToList();

        return new ViewManagerOverviewResponse(schedules);
    }
}