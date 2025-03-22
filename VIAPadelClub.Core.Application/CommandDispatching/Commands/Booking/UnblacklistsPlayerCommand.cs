using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.Booking;

public class UnblacklistsPlayerCommand
{
    internal Guid PlayerId { get; private set; }
    
    private UnblacklistsPlayerCommand(Guid playerId)
    {
        PlayerId = playerId;
    }
    
    public static Result<UnblacklistsPlayerCommand> Create(string playerId)
    {
        var playerIdGuid = Guid.Parse(playerId);
        
        var unblacklistsPlayerCommand = new UnblacklistsPlayerCommand(playerIdGuid);
        return Result<UnblacklistsPlayerCommand>.Ok(unblacklistsPlayerCommand);
    }
}