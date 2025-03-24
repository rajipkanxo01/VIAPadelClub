using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Booking;

public class PlayerCancelsBookingHandler: ICommandHandler<PlayerCancelsBookingCommand>
{
    private readonly IDailyScheduleRepository _dailyScheduleRepository;
    private readonly ITimeProvider _timeProvider;
    private readonly IDateProvider _dateProvider;
    private readonly IUnitOfWork _unitOfWork;

    public PlayerCancelsBookingHandler(IDailyScheduleRepository dailyScheduleRepository, IDateProvider dateProvider, ITimeProvider timeProvider, IUnitOfWork unitOfWork)
    {
        _dailyScheduleRepository = dailyScheduleRepository;
        _dateProvider = dateProvider;
        _timeProvider = timeProvider;
        _unitOfWork = unitOfWork;
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

        _unitOfWork.SaveChangesAsync();

        return Task.FromResult(Result.Ok());
    }
}