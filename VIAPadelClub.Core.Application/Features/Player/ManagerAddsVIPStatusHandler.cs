using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Booking;

public class ChangePlayerToVipCommandHandler : ICommandHandler<ChangePlayerToVipStatusCommand>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePlayerToVipCommandHandler(IPlayerRepository playerRepository, IUnitOfWork unitOfWork)
    {
        _playerRepository = playerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(ChangePlayerToVipStatusCommand statusCommand)
    {
        var playerResult = await _playerRepository.GetAsync(statusCommand.Email);
        if (!playerResult.Success)
        {
            return Result.Fail(playerResult.ErrorMessage);
        }

        var player = playerResult.Data;

        var vipResult = player.ChangeToVIPStatus();
        if (!vipResult.Success)
        {
            return Result.Fail(vipResult.ErrorMessage);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }
}