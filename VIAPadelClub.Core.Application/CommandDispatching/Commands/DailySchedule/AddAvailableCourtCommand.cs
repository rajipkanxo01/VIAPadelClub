using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;

public class AddAvailableCourtCommand
{
    internal Guid DailyScheduleId { get; private set; }
    internal Court Court { get; private set; }
    
    private AddAvailableCourtCommand(Guid dailyScheduleId, Court court)
    {
        DailyScheduleId = dailyScheduleId;
        Court = court;
    }
    
    public static Result<AddAvailableCourtCommand> Create(string dailyScheduleIdStr, string courtStr)
    {
        var courtNameResult = CourtName.Create(courtStr);

        if (!courtNameResult.Success)
        {
            return Result<AddAvailableCourtCommand>.Fail(courtNameResult.ErrorMessage);
        }
        
        var courtResult= Court.Create(courtNameResult.Data);
        
        if (!Guid.TryParse(dailyScheduleIdStr, out var dailyScheduleId))
        {
            return Result<AddAvailableCourtCommand>.Fail(DailyScheduleError.InvalidScheduleIdFormatWhileParsing()._message);
        }
        
        var command = new AddAvailableCourtCommand(dailyScheduleId, courtResult.Data);
        return Result<AddAvailableCourtCommand>.Ok(command);
    }
}