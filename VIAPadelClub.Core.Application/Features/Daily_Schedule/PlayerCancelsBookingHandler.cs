using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Daily_Schedule;

public class PlayerCancelsBookingHandler: ICommandHandler<PlayerCancelsBookingCommand>
{
    private readonly IDailyScheduleRepository _dailyScheduleRepository;
    private readonly ITimeProvider _timeProvider;
    private readonly IDateProvider _dateProvider;

    public PlayerCancelsBookingHandler(IDailyScheduleRepository dailyScheduleRepository, IDateProvider dateProvider, ITimeProvider timeProvider)
    {
        _dailyScheduleRepository = dailyScheduleRepository;
        _dateProvider = dateProvider;
        _timeProvider = timeProvider;
    }

    public Task<Result> HandleAsync(PlayerCancelsBookingCommand command)
    {
        var dailyScheduleResult = _dailyScheduleRepository.GetAsync(command.DailyScheduleId).Result;

        if (!dailyScheduleResult.Success)
        {
            return Task.FromResult(Result.Fail(dailyScheduleResult.ErrorMessage));
        }

        var dailySchedule = dailyScheduleResult.Data;

        var bookingResult = dailySchedule.CancelBooking(command.BookingId, _dateProvider, _timeProvider, command.PlayerMakingCancel );

        if (!bookingResult.Success)
        {
            return Task.FromResult(Result.Fail(bookingResult.ErrorMessage));
        }
        

        return Task.FromResult(Result.Ok());
    }
}