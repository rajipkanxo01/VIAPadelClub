using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;

public class CreateBookingCommand
{
    internal Guid DailyScheduleId { get; private set; }
    internal Email BookedBy { get; private set; }
    internal TimeOnly StartTime { get; private set; }
    internal TimeOnly EndTime { get; private set; }
    internal Court Court { get; private set; }
    
    private CreateBookingCommand(Guid dailyScheduleId, Email bookedBy, TimeOnly startTime, TimeOnly endTime, Court court)
    {
        DailyScheduleId = dailyScheduleId;
        BookedBy = bookedBy;
        StartTime = startTime;
        EndTime = endTime;
        Court = court;
    }
    
    public static Result<CreateBookingCommand> Create(string dailyScheduleIdStr, string bookedByStr, string startTimeStr, string endTimeStr, string courtStr)
    {
        var emailResult = Email.Create(bookedByStr);
        var courtNameResult =CourtName.Create(courtStr);

        if (!courtNameResult.Success)
        {
            return Result<CreateBookingCommand>.Fail(courtNameResult.ErrorMessage);
        }
        
        var courtResult= Court.Create(courtNameResult.Data);
        
        if (!Guid.TryParse(dailyScheduleIdStr, out var dailyScheduleId))
        {
            return Result<CreateBookingCommand>.Fail(DailyScheduleError.InvalidScheduleIdFormatWhileParsing()._message);
        }
        
        if (!TimeOnly.TryParse(startTimeStr, out var startTime))
        {
            return Result<CreateBookingCommand>.Fail(DailyScheduleError.InvalidTimeformatWhileParsing()._message);
        }
        
        if (!TimeOnly.TryParse(endTimeStr, out var endTime))
        {
            return Result<CreateBookingCommand>.Fail(DailyScheduleError.InvalidTimeformatWhileParsing()._message);
        }

        if (!emailResult.Success)
        {
            return Result<CreateBookingCommand>.Fail(emailResult.ErrorMessage);
        }
        
        var command = new CreateBookingCommand(dailyScheduleId, emailResult.Data, startTime, endTime, courtResult.Data);
        return Result<CreateBookingCommand>.Ok(command);
    }
}