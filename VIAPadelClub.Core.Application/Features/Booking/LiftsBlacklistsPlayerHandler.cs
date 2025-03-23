using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Booking;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Booking;

public class LiftsBlacklistsPlayerHandler : ICommandHandler<LiftsBlacklistsPlayerCommand>
{
    private readonly IPlayerRepository _playerRepository;

    public LiftsBlacklistsPlayerHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public Task<Result> HandleAsync(LiftsBlacklistsPlayerCommand command)
    {
        var result = _playerRepository.GetAsync(command.PlayerId).Result;

        if (!result.Success)
        {
            return Task.FromResult(Result.Fail(result.ErrorMessage));
        }

        var liftBlacklistResult = result.Data.LiftBlacklist();
        return Task.FromResult(liftBlacklistResult);
    }
}