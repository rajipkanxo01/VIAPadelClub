using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.ObjectMapper;
using VIAPadelClub.Presentation.WebApi.Endpoints.Queries;

namespace VIAPadelClub.Presentation.WebApi.ObjectMapping;

public class PlayerScheduleAnswerToPlayerScheduleViewResponseMapping : 
    IMappingConfig<PlayerScheduleOverview.Answer, PlayerScheduleViewResponse>
{
    public PlayerScheduleViewResponse Map(PlayerScheduleOverview.Answer input)
    {
        var schedules = input.Schedules.Select(schedule =>
            new PlayerSchedule(
                schedule.Id,
                schedule.Date,
                schedule.Status
            )).ToList();

        return new PlayerScheduleViewResponse(schedules);
    }
}