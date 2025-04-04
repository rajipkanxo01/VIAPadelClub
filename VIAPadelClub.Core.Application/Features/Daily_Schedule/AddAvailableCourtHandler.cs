using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Daily_Schedule;

public class AddAvailableCourtHandler:ICommandHandler<AddAvailableCourtCommand>
{
    private readonly IDailyScheduleRepository _dailyScheduleRepository;
    private readonly IDateProvider _dateProvider;
    private readonly IScheduleFinder _scheduleFinder;
    
    public AddAvailableCourtHandler(IDailyScheduleRepository dailyScheduleRepository, IDateProvider dateProvider, IScheduleFinder scheduleFinder)
    {
        _dailyScheduleRepository = dailyScheduleRepository;
        _dateProvider = dateProvider;
        _scheduleFinder = scheduleFinder;
    }
    
    public Task<Result> HandleAsync(AddAvailableCourtCommand command)
    {
        var dailyScheduleResult = _dailyScheduleRepository.GetAsync(command.DailyScheduleId).Result;

        if (!dailyScheduleResult.Success)
        {
            return Task.FromResult(Result.Fail(dailyScheduleResult.ErrorMessage));
        }

        var result = dailyScheduleResult.Data.AddAvailableCourt(command.Court,_dateProvider, _scheduleFinder);
        return Task.FromResult(result);
    }
}