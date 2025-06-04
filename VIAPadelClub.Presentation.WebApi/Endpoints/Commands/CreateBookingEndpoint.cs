using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.Dispatcher;
using VIAPadelClub.Presentation.WebApi.Endpoints.Common;

namespace VIAPadelClub.Presentation.WebApi.Endpoints.Commands;

public class CreateBookingEndpoint(ICommandDispatcher commandDispatcher)
    : EndpointBase
        .ApiEndpoint
        .WithRequest<CreateBookingRequest>
        .AndResult<NoContent, BadRequest<string>>
{
    [HttpPost("schedules/createBooking")]
    public override async Task<Results<NoContent, BadRequest<string>>> HandleAsync(CreateBookingRequest request)
    {
        var commandResult = CreateBookingCommand.Create(
            request.RequestBody.dailyScheduleId,
            request.RequestBody.bookedBy,
            request.RequestBody.startTime,
            request.RequestBody.endTime,
            request.RequestBody.courtName
            );

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

public record CreateBookingRequest(
    [FromBody] CreateBookingRequest.Body RequestBody)
{
    public record Body(string dailyScheduleId, string bookedBy, string startTime, string endTime, string courtName); 
}