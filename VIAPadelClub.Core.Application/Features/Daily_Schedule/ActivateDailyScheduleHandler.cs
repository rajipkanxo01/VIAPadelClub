using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Daily_Schedule;

public class ActivateDailyScheduleHandler: ICommandHandler<ActivateDailyScheduleCommand>
{
    private readonly IDailyScheduleRepository _dailyScheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateProvider _dateProvider;
    
    public ActivateDailyScheduleHandler(IDailyScheduleRepository dailyScheduleRepository, IUnitOfWork unitOfWork, IDateProvider dateProvider)
    {
        _dailyScheduleRepository = dailyScheduleRepository;
        _unitOfWork = unitOfWork;
        _dateProvider = dateProvider;
    }
    public Task<Result> HandleAsync(ActivateDailyScheduleCommand command)
    {
        var dailyScheduleResult = _dailyScheduleRepository.GetAsync(command.ScheduleId).Result;

        if (!dailyScheduleResult.Success)
        {
            return Task.FromResult(Result.Fail(dailyScheduleResult.ErrorMessage));
        }

        var result = dailyScheduleResult.Data.Activate(_dateProvider);
        _unitOfWork.SaveChangesAsync();
        
        return Task.FromResult(result);
    }
}