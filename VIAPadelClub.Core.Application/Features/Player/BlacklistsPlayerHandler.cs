using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Booking;

public class BlacklistsPlayerHandler: ICommandHandler<BlacklistsPlayerCommand>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IDailyScheduleRepository _dailyScheduleRepository;
    
    public BlacklistsPlayerHandler(IPlayerRepository playerRepo, IDailyScheduleRepository dailyScheduleRepository)
    {
        _playerRepository = playerRepo;
        _dailyScheduleRepository = dailyScheduleRepository;
    }

    public async Task<Result> HandleAsync(BlacklistsPlayerCommand command)
    {
        var playerResult = await _playerRepository.GetAsync(command.PlayerId);
        if (!playerResult.Success)
        {
            return Result.Fail(playerResult.ErrorMessage);
        }

        var player = playerResult.Data;

        var scheduleResult = await _dailyScheduleRepository.GetAllAsync();
        if (!scheduleResult.Success)
        {
            return Result.Fail(scheduleResult.ErrorMessage);
        }

        var schedules = scheduleResult.Data;

        var result = player.Blacklist(schedules);

        if (!result.Success)
        {
            return result;
        }

        return Result.Ok();
    }
}