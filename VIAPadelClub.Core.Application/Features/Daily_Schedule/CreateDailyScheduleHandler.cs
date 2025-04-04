using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Daily_Schedule;

public class CreateDailyScheduleHandler : ICommandHandler<CreateDailyScheduleCommand>
{
    private readonly IDailyScheduleRepository _dailyScheduleRepository;
    private readonly IDateProvider _dateProvider;

    public CreateDailyScheduleHandler(IDailyScheduleRepository dailyScheduleRepository, IDateProvider dateProvider)
    {
        _dailyScheduleRepository = dailyScheduleRepository;
        _dateProvider = dateProvider;
    }

    public async Task<Result> HandleAsync(CreateDailyScheduleCommand command)
    {
        var scheduleResult = Domain.Aggregates.DailySchedules.DailySchedule.CreateSchedule(_dateProvider);

        if (!scheduleResult.Success)
        {
            return Result.Fail(scheduleResult.ErrorMessage);
        }

        await _dailyScheduleRepository.AddAsync(scheduleResult.Data);

        return Result.Ok();
    }
}