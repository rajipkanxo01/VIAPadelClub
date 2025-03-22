using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.Booking;

public class LiftsBlacklistsPlayerCommand
{
    internal Email PlayerId { get; private set; }
    
    private LiftsBlacklistsPlayerCommand(Email playerId)
    {
        PlayerId = playerId;
    }
    
    public static Result<LiftsBlacklistsPlayerCommand> Create(string playerEmailStr)
    {
        var result = Email.Create(playerEmailStr);

        if (!result.Success)
        {
            return Result<LiftsBlacklistsPlayerCommand>.Fail(result.ErrorMessage);
        }

        var unblacklistsPlayerCommand = new LiftsBlacklistsPlayerCommand(result.Data);
        return Result<LiftsBlacklistsPlayerCommand>.Ok(unblacklistsPlayerCommand);
    }
}