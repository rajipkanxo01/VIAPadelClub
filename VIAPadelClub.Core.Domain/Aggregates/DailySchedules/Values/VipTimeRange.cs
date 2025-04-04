using VIAPadelClub.Core.Domain.Common.BaseClasses;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;

public class VipTimeRange : ValueObject
{
    internal TimeOnly Start { get; private set; }
    internal TimeOnly End { get; private set; }

    private VipTimeRange() { } // Required by EF Core

    protected VipTimeRange(TimeOnly start, TimeOnly end)
    {
        Start = start;
        End = end;
    }

    public static Result<VipTimeRange> Create(TimeOnly start, TimeOnly end)
    {
        var vipTimeRange = new VipTimeRange(start, end);
        return Result<VipTimeRange>.Ok(vipTimeRange);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }
}