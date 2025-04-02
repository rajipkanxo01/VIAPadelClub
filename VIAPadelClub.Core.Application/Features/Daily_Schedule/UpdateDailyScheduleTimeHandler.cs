using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Daily_Schedule;

public class UpdateDailyScheduleTimeHandler : ICommandHandler<UpdateDailyScheduleTimeCommand>
{
    private readonly IDailyScheduleRepository _dailyScheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateProvider _dateProvider;

    public UpdateDailyScheduleTimeHandler(
        IDailyScheduleRepository dailyScheduleRepository,
        IUnitOfWork unitOfWork,
        IDateProvider dateProvider)
    {
        _dailyScheduleRepository = dailyScheduleRepository;
        _unitOfWork = unitOfWork;
        _dateProvider = dateProvider;
    }

    public async Task<Result> HandleAsync(UpdateDailyScheduleTimeCommand command)
    {
        var scheduleResult = await _dailyScheduleRepository.GetAsync(command.ScheduleId);
        if (!scheduleResult.Success)
        {
            return Result.Fail(scheduleResult.ErrorMessage);
        }

        var schedule = scheduleResult.Data;

        var updateResult = schedule.UpdateScheduleDateAndTime(
            command.NewDate,
            command.NewStartTime,
            command.NewEndTime,
            _dateProvider
        );

        if (!updateResult.Success)
        {
            return Result.Fail(updateResult.ErrorMessage);
        }
        
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}