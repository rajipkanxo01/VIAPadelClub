using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Booking;

public class QuarantinesPlayerCommandHandler : ICommandHandler<QuarantinesPlayerCommand>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IDailyScheduleRepository _dailyScheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public QuarantinesPlayerCommandHandler(
        IPlayerRepository playerRepository,
        IDailyScheduleRepository dailyScheduleRepository,
        IUnitOfWork unitOfWork)
    {
        _playerRepository = playerRepository;
        _dailyScheduleRepository = dailyScheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(QuarantinesPlayerCommand command)
    {
        // Get player by email
        var playerResult = await _playerRepository.GetAsync(command.Email);
        if (!playerResult.Success)
        {
            return Result.Fail(playerResult.ErrorMessage);
        }

        var player = playerResult.Data;

        // Get all daily schedules
        var scheduleResult = await _dailyScheduleRepository.GetAllAsync();
        if (!scheduleResult.Success)
        {
            return Result.Fail(scheduleResult.ErrorMessage);
        }

        var schedules = scheduleResult.Data;

        // Quarantine the player
        var quarantineResult = player.Quarantine(command.StartDate, schedules);
        if (!quarantineResult.Success)
        {
            return Result.Fail(quarantineResult.ErrorMessage);
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }
}