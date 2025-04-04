using VIAPadelClub.Core.Tools.OperationResult;
using System;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;

public class ChangePlayerToVipStatusCommand
{
    internal Email Email { get; private set; }

    private ChangePlayerToVipStatusCommand(Email email)
    {
        Email = email;
    }

    public static Result<ChangePlayerToVipStatusCommand> Create(string emailStr)
    {
        var emailResult = Email.Create(emailStr);
        if (!emailResult.Success)
        {
            return Result<ChangePlayerToVipStatusCommand>.Fail(emailResult.ErrorMessage);
        }

        return Result<ChangePlayerToVipStatusCommand>.Ok(new ChangePlayerToVipStatusCommand(emailResult.Data));
    }
}