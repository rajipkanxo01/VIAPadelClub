using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.Dispatcher;
using VIAPadelClub.Presentation.WebApi.Endpoints.Common;

namespace VIAPadelClub.Presentation.WebApi.Endpoints.Commands;

public class AddCourtToDailyScheduleEndpoint(ICommandDispatcher commandDispatcher)
    : EndpointBase
        .ApiEndpoint
        .WithRequest<AddCourtToScheduleRequest>
        .AndResult<NoContent, BadRequest<string>>
{
    [HttpPost("schedules/addCourt")]
    public override async Task<Results<NoContent, BadRequest<string>>>
        HandleAsync(AddCourtToScheduleRequest request)
    {
        var commandResult = AddAvailableCourtCommand.Create(
            request.RequestBody.ScheduleId,
            request.RequestBody.CourtName);

        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ErrorMessage);
        }
        
        var result = await commandDispatcher.DispatchAsync(commandResult.Data);

        if (!result.Success)
        {
            return TypedResults.BadRequest(result.ErrorMessage);
        }

        return TypedResults.NoContent();
    }
}

public record AddCourtToScheduleRequest(
    [FromBody] AddCourtToScheduleRequest.Body RequestBody)
{
    public record Body(string CourtName, string ScheduleId); 
}