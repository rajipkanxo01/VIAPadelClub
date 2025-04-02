using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Daily_Schedule;

public class DeleteDailyScheduleCommandHandler : ICommandHandler<DeleteDailyScheduleCommand>
{
    private readonly IDailyScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateProvider _dateProvider;
    private readonly ITimeProvider _timeProvider;

    public DeleteDailyScheduleCommandHandler(
        IDailyScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork,
        IDateProvider dateProvider,
        ITimeProvider timeProvider)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
        _dateProvider = dateProvider;
        _timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(DeleteDailyScheduleCommand command)
    {
        var scheduleResult = await _scheduleRepository.GetAsync(command.ScheduleId);
        if (!scheduleResult.Success)
        {
            return Result.Fail(scheduleResult.ErrorMessage);
        }

        var schedule = scheduleResult.Data;
        var deleteResult = schedule.DeleteSchedule(_dateProvider, _timeProvider);

        if (!deleteResult.Success)
        {
            return Result.Fail(deleteResult.ErrorMessage);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }
}