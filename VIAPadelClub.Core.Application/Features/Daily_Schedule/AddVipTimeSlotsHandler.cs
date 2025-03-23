using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Daily_Schedule;

public class AddVipTimeSlotsHandler : ICommandHandler<AddVipTimeSlotCommand>
{
    private readonly IDailyScheduleRepository _dailyScheduleRepository;

    public AddVipTimeSlotsHandler(IDailyScheduleRepository dailyScheduleRepository)
    {
        _dailyScheduleRepository = dailyScheduleRepository;
    }

    public Task<Result> HandleAsync(AddVipTimeSlotCommand command)
    {
        var dailyScheduleResult = _dailyScheduleRepository.GetAsync(command.DailyScheduleId).Result;

        if (!dailyScheduleResult.Success)
        {
            return Task.FromResult(Result.Fail(dailyScheduleResult.ErrorMessage));
        }

        var result = dailyScheduleResult.Data.AddVipTimeSlots(command.StartTime, command.EndTime);
        return Task.FromResult(result);
    }
}