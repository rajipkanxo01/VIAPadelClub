using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Daily_Schedule;

public class CreateBookingHandler : ICommandHandler<CreateBookingCommand>
{
    private readonly IDailyScheduleRepository _dailyScheduleRepository;
    private readonly IDateProvider _dateProvider;
    private readonly IPlayerFinder _playerFinder;
    private readonly IScheduleFinder _scheduleFinder;

    public CreateBookingHandler(IDailyScheduleRepository dailyScheduleRepository, IDateProvider dateProvider, IPlayerFinder playerFinder, IScheduleFinder scheduleFinder)
    {
        _dailyScheduleRepository = dailyScheduleRepository;
        _dateProvider = dateProvider;
        _playerFinder = playerFinder;
        _scheduleFinder = scheduleFinder;
    }

    public async Task<Result> HandleAsync(CreateBookingCommand command)
    {
        var scheduleResult = _dailyScheduleRepository.GetAsync(command.DailyScheduleId).Result;
        
        if (!scheduleResult.Success)
        {
            return await Task.FromResult(Result.Fail(scheduleResult.ErrorMessage));
        }
        
        var schedule = scheduleResult.Data;
        var bookingResult = schedule.BookCourt(command.BookedBy,command.Court, command.StartTime, command.EndTime, _dateProvider, _playerFinder, _scheduleFinder);
        
        if (!bookingResult.Success)
        {
            return await Task.FromResult(Result.Fail(bookingResult.ErrorMessage));
        }

        return await Task.FromResult(Result.Ok());
    }
}