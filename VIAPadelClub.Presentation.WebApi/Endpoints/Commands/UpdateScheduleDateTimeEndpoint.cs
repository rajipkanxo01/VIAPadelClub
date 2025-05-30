using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.Dispatcher;
using VIAPadelClub.Presentation.WebApi.Endpoints.Common;

namespace VIAPadelClub.Presentation.WebApi.Endpoints.Commands;

public record UpdateScheduleDateTimeRequest(string ScheduleId, string Date, string StartTime, string EndTime);
public record UpdateScheduleDateTimeRequestBody();

public class UpdateScheduleDateTimeEndpoint(ICommandDispatcher dispatcher) : EndpointBase.ApiEndpoint.WithRequest<UpdateScheduleDateTimeRequest>.AndResult<NoContent, BadRequest<string>>
{
    [HttpPut("dailySchedule/update")]
    public override async Task<Results<NoContent, BadRequest<string>>> HandleAsync([FromBody] UpdateScheduleDateTimeRequest request)
    {
        var commandResult = UpdateDailyScheduleTimeCommand.Create(
            request.ScheduleId,
            request.Date,
            request.StartTime,
            request.EndTime
        );

        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ErrorMessage);
        }

        var dispatchResult = await dispatcher.DispatchAsync(commandResult.Data);
        if (!dispatchResult.Success)
        {
            return TypedResults.BadRequest(dispatchResult.ErrorMessage);
        }

        return TypedResults.NoContent();
    }
}