using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;

public class RemoveAvailableCourtCommand
{
    internal Guid DailyScheduleId { get; private set; }
    internal Court Court { get; private set; }
    internal TimeOnly TimeOfRemoval { get; private set; }
    
    private RemoveAvailableCourtCommand(Guid dailyScheduleId, Court court, TimeOnly timeOfRemoval)
    {
        DailyScheduleId = dailyScheduleId;
        Court = court;
        TimeOfRemoval = timeOfRemoval;
    }
    
    public static Result<RemoveAvailableCourtCommand> Create(string dailyScheduleIdStr, string courtStr, string timeOfRemovalStr)
    {
        var courtNameResult = CourtName.Create(courtStr);

        if (!courtNameResult.Success)
        {
            return Result<RemoveAvailableCourtCommand>.Fail(courtNameResult.ErrorMessage);
        }
        
        var courtResult= Court.Create(courtNameResult.Data);
        
        if (!Guid.TryParse(dailyScheduleIdStr, out var dailyScheduleId))
        {
            return Result<RemoveAvailableCourtCommand>.Fail(DailyScheduleError.InvalidScheduleIdFormatWhileParsing()._message);
        }
        
        if (!TimeOnly.TryParse(timeOfRemovalStr, out var timeOfRemoval))
        {
            return Result<RemoveAvailableCourtCommand>.Fail(DailyScheduleError.InvalidTimeformatWhileParsing()._message);
        }

        var command = new RemoveAvailableCourtCommand(dailyScheduleId, courtResult.Data, timeOfRemoval);
        return Result<RemoveAvailableCourtCommand>.Ok(command);
    }
}