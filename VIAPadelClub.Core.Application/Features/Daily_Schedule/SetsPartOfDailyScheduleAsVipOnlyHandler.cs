using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features;

public class SetsPartOfDailyScheduleAsVipOnlyHandler : ICommandHandler<SetsPartOfDailyScheduleAsVipOnlyCommand>
{
    public Task<Result> HandleAsync(SetsPartOfDailyScheduleAsVipOnlyCommand command)
    {
        throw new NotImplementedException();
    }
}