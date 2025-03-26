using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Daily_Schedule;

public class RemoveAvailableCourtHandler: ICommandHandler<RemoveAvailableCourtCommand>
{
    private readonly IDailyScheduleRepository _dailyScheduleRepository;
    private readonly IDateProvider _dateProvider;
    private readonly IUnitOfWork _unitOfWork;
    
    public RemoveAvailableCourtHandler(IDailyScheduleRepository dailyScheduleRepository, IDateProvider dateProvider, IUnitOfWork unitOfWork)
    {
        _dailyScheduleRepository = dailyScheduleRepository;
        _dateProvider = dateProvider;
        _unitOfWork = unitOfWork;
    }

    public Task<Result> HandleAsync(RemoveAvailableCourtCommand command)
    {
        var dailyScheduleResult = _dailyScheduleRepository.GetAsync(command.DailyScheduleId).Result;

        if (!dailyScheduleResult.Success)
        {
            return Task.FromResult(Result.Fail(dailyScheduleResult.ErrorMessage));
        }

        var result = dailyScheduleResult.Data.RemoveAvailableCourt(command.Court,_dateProvider,command.TimeOfRemoval);
        _unitOfWork.SaveChangesAsync();
        return Task.FromResult(result);
    }
}