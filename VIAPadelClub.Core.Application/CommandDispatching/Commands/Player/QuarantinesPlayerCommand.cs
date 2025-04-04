using VIAPadelClub.Core.Application.Features.Booking;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;

public class QuarantinesPlayerCommand
{
    internal Email Email { get; private set; }
    internal DateOnly StartDate { get; private set; }

    private QuarantinesPlayerCommand(Email email, DateOnly startDate)
    {
        Email = email;
        StartDate = startDate;
    }

    public static Result<QuarantinesPlayerCommand> Create(string emailStr, string startDateStr)
    {
        var emailResult = Email.Create(emailStr);
        if (!emailResult.Success)
            return Result<QuarantinesPlayerCommand>.Fail(emailResult.ErrorMessage);

        if (!DateOnly.TryParse(startDateStr, out var startDate))
            return Result<QuarantinesPlayerCommand>.Fail(DailyScheduleError.InvalidDateformatWhileParsing()._message);

        return Result<QuarantinesPlayerCommand>.Ok(new QuarantinesPlayerCommand(emailResult.Data, startDate));
    }
}