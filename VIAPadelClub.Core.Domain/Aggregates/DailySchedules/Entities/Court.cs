using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;

public class Court
{
    internal CourtName Name { get; }

    private Court(CourtName name)
    {
        Name = name;
    }

    public static Result<Court> Create(CourtName name)
    {
        var court = new Court(name);
        return Result<Court>.Ok(court);
    }
}