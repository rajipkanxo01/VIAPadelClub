using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Booking;

public class BlacklistsPlayerHandler: ICommandHandler<BlacklistsPlayerCommand>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IScheduleFinder _scheduleFinder;
    
    public BlacklistsPlayerHandler(IPlayerRepository playerRepo, IScheduleFinder scheduleFinder)
    {
        _playerRepository = playerRepo;
        _scheduleFinder = scheduleFinder;
    }

    public async Task<Result> HandleAsync(BlacklistsPlayerCommand command)
    {
        var player = (await _playerRepository.GetAsync(command.PlayerId)).Data;

        var result = player.Blacklist(_scheduleFinder);

        if (!result.Success)
        {
            return result;
        }

        return Result.Ok();
    }
}