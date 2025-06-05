using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.ObjectMapper;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

namespace VIAPadelClub.Presentation.WebApi.ObjectMapping;

public class ScheduleToScheduleViewMapping : IMappingConfig<DailySchedule, ViewManagerOverview.ScheduleView>
{
    public ViewManagerOverview.ScheduleView Map(DailySchedule input)
    {
            return new ViewManagerOverview.ScheduleView(
                input.ScheduleId,
                input.ScheduleDate,
                input.Status,
                input.Courts.Count
            );
        }
}