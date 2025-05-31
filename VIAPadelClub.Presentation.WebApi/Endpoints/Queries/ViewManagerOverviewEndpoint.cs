using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.QueryContracts.QueryDispatching;
using VIAPadelClub.Core.Tools.ObjectMapper;
using VIAPadelClub.Presentation.WebApi.Endpoints.Common;

namespace VIAPadelClub.Presentation.WebApi.Endpoints.Queries;

public record ViewManagerOverviewRequest([FromQuery] string MonthName);
public record ViewManagerOverviewResponse(List<Schedule> Schedules);
public record Schedule(
    string Id,
    string Date,
    string Status,
    int CourtCount
);

public class ViewManagerOverviewEndpoint(IQueryDispatcher queryDispatcher, IMapper mapper) : EndpointBase.ApiEndpoint
    .WithRequest<ViewManagerOverviewRequest>
    .AndResult<Ok<ViewManagerOverviewResponse>, BadRequest<string>>
{
    
    [HttpGet("manager/overview")]
    public override async Task<Results<Ok<ViewManagerOverviewResponse>, BadRequest<string>>> HandleAsync([FromQuery] ViewManagerOverviewRequest request)
    {
        
        if (string.IsNullOrWhiteSpace(request.MonthName))
            return TypedResults.BadRequest("MonthName must be provided.");

        
        var query = new ViewManagerOverview.Query(request.MonthName);
        
        var dispatchResult = await queryDispatcher.DispatchAsync(query);
        if (!dispatchResult.Success)
        {
            return TypedResults.BadRequest(dispatchResult.ErrorMessage);
        }
        
        var answer = dispatchResult.Data;
        var response = mapper.Map<ViewManagerOverviewResponse>(answer);
        return TypedResults.Ok(response);
    }
}