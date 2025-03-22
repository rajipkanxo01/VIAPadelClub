using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;

public class CreateDailyScheduleCommand
{
    private CreateDailyScheduleCommand()
    {
    }

    public static Result<CreateDailyScheduleCommand> Create()
    {
        var createDailyScheduleCommand = new CreateDailyScheduleCommand();
        return Result<CreateDailyScheduleCommand>.Ok(createDailyScheduleCommand);
    }
}