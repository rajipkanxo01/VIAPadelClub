using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.QueryContracts.QueryDispatching;
using VIAPadelClub.Core.Tools.ObjectMapper;
using VIAPadelClub.Presentation.WebApi.Endpoints.Common;

namespace VIAPadelClub.Presentation.WebApi.Endpoints.Queries;

public record PlayerScheduleViewRequest([FromQuery] string MonthName);
public record PlayerScheduleViewResponse(List<PlayerSchedule> Schedules);
public record PlayerSchedule(
    string Id,
    string Date,
    string Status
);

public class PlayerScheduleViewEndpoint(IQueryDispatcher queryDispatcher, IMapper mapper) : EndpointBase.ApiEndpoint
    .WithRequest<PlayerScheduleViewRequest>
    .AndResult<Ok<PlayerScheduleViewResponse>, BadRequest<string>>
{
    
    [HttpGet("player/schedule")]
    public override async Task<Results<Ok<PlayerScheduleViewResponse>, BadRequest<string>>> HandleAsync([FromQuery] PlayerScheduleViewRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MonthName))
            return TypedResults.BadRequest("MonthName must be provided.");

        var query = new PlayerScheduleOverview.Query(request.MonthName);

        var dispatchResult = await queryDispatcher.DispatchAsync(query);
        if (!dispatchResult.Success)
        {
            return TypedResults.BadRequest(dispatchResult.ErrorMessage);
        }

        var answer = dispatchResult.Data;
        var response = mapper.Map<PlayerScheduleViewResponse>(answer);
        return TypedResults.Ok(response);
    }
}