using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.Dispatcher;
using VIAPadelClub.Core.Tools.OperationResult;
using VIAPadelClub.Presentation.WebApi.Endpoints.Common;

namespace VIAPadelClub.Presentation.WebApi.Endpoints.Commands;

public class CreateDailyScheduleEndpoint(ICommandDispatcher commandDispatcher) : EndpointBase.ApiEndpoint.WithoutRequest.AndResult<NoContent, BadRequest<string>>
{
    
    [HttpPost("dailySchedule/create")]
    public override async Task<Results<NoContent, BadRequest<string>>> HandleAsync()
    {
        var commandResult = CreateDailyScheduleCommand.Create();
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ErrorMessage);
        }

        var dispatchResult = await commandDispatcher.DispatchAsync(commandResult.Data);
        if (!dispatchResult.Success)
        {
            return TypedResults.BadRequest(dispatchResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
    }
}